from typing import Any, Self

import ray
import torch
from attr import frozen
from loguru import logger

from core.src.communication.application.godot_app_handler import GodotAppHandler
from core.src.communication.pipe_handler import PipeHandler
from core.src.managers.simulation_manager import SimulationManager
from core.src.managers.storage_manager import StorageManager
from core.src.managers.training_manager import TrainingManager
from core.src.settings.app_settings import APP_REGISTRY, AppSettingsSchema
from core.src.settings.core_settings import CoreSettingsSchema
from core.src.settings.settings import Environment, TrainingSettings, get_settings
from core.src.setup import configure_logging
from core.src.utils.registry.shared_registry import SharedRegistry


@frozen
class App:
    app_settings: AppSettingsSchema
    registry: SharedRegistry[type, Any]

    @classmethod
    def assemble(cls) -> Self:
        configure_logging()
        cls.startup_ray()  # Ray has to be started before creating a registry
        registry: SharedRegistry[type, Any] = SharedRegistry(APP_REGISTRY)
        app_settings = AppSettingsSchema()
        registry.put(AppSettingsSchema, app_settings)
        return cls(app_settings, registry)

    def run(self):
        if self.app_settings.work_environment.env == Environment.CLI:
            core_settings = CoreSettingsSchema()
            self.registry.put(CoreSettingsSchema, core_settings)
            settings = get_settings()
            storage_settings = settings.storage
            storage_manager = StorageManager(storage_settings)
            self.train(settings.training, storage_manager)
            return

        pipe_name = self.app_settings.work_environment.pipe_name
        pipe_handler = PipeHandler(pipe_name)

        with GodotAppHandler(pipe_handler) as handler:
            raw_settings = handler.receive()  # hmm?
            core_settings = CoreSettingsSchema.model_validate_json(raw_settings)
            logger.info(f"Core settings created:\n{core_settings}")
            handler.send(bytes(1))
            self.registry.put(CoreSettingsSchema, core_settings)
            settings = get_settings()
            storage_settings = settings.storage
            storage_manager = StorageManager(storage_settings)

            self.inference(storage_manager)  # inference until told to stop
            self.train(settings.training, storage_manager)

    @classmethod
    def startup_ray(cls):
        ray.shutdown()
        ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, include_dashboard=True, num_gpus=1)

        if torch.cuda.is_available():
            logger.info(f"GPU is available, using: {torch.cuda.get_device_name(0)}")
        else:
            logger.info("GPU is not available, using cpu")

    def train(self, training_settings: TrainingSettings, storage_manager: StorageManager):
        training_manager = TrainingManager(
            training_settings=training_settings, storage_manager=storage_manager, tuner=True
        )
        training_manager.train()

    def inference(self, storage_manager: StorageManager):
        simulation_manager = SimulationManager(storage_manager=storage_manager)
        simulation_manager.run(num_episodes=1)
