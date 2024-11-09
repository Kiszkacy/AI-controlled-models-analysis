from typing import Any, Self

import ray
import torch
from attr import frozen
from loguru import logger

from core.src.communication.application.godot_app_handler import GodotAppHandler
from core.src.communication.pipe_handler import PipeHandler
from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.settings.app_settings import APP_REGISTRY, AppSettings, Environment
from core.src.settings.core_settings import CoreSettings
from core.src.setup import configure_logging
from core.src.utils.registry.shared_registry import SharedRegistry
from core.src.utils.training_handler import TrainingHandler


@frozen
class App:
    app_settings: AppSettings
    registry: SharedRegistry[type, Any]

    @classmethod
    def assemble(cls) -> Self:
        configure_logging()
        cls.startup_ray()  # Ray has to be started before creating a registry
        registry: SharedRegistry[type, Any] = SharedRegistry(APP_REGISTRY)
        app_settings = AppSettings()
        registry.put(AppSettings, app_settings)
        return cls(app_settings, registry)

    def run(self):
        if self.app_settings.work_environment.env == Environment.DEVELOPMENT:
            core_settings = CoreSettings()
            self.registry.put(CoreSettings, core_settings)

            self.train()
            return

        pipe_name = self.app_settings.communication.pipe_name
        pipe_handler = PipeHandler(pipe_name)

        with GodotAppHandler(pipe_handler) as handler:
            raw_settings = handler.receive()  # hmm?
            core_settings = CoreSettings.model_validate_json(raw_settings)
            self.registry.put(CoreSettings, core_settings)

            # For now just training?
            self.train()

    @classmethod
    def startup_ray(cls):
        ray.shutdown()
        ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, include_dashboard=True, num_gpus=1)

        if torch.cuda.is_available():
            logger.info(f"GPU is available, using: {torch.cuda.get_device_name(0)}")
        else:
            logger.info("GPU is not available, using cpu")

    def train(self):
        training_handler = TrainingHandler("PPO", environment_cls=GodotServerEnvironment)
        training_handler.train()

    def inference(self): ...
