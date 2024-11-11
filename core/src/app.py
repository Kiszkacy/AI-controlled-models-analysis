import threading
from typing import Any, Self

import ray
import torch
from attr import frozen
from loguru import logger
from ray.rllib.algorithms import PPOConfig
from ray.rllib.algorithms.ppo import PPOTorchPolicy
from ray.rllib.models import ModelCatalog

from core.src.communication.application.godot_app_handler import GodotAppHandler
from core.src.communication.godot_handler import GodotHandler
from core.src.communication.pipe_handler import PipeHandler
from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.environments.godot_inference_environment import GodotServerInferenceEnvironment
from core.src.policies.custom_model import CustomModel
from core.src.settings.app_settings import APP_REGISTRY, AppSettingsSchema
from core.src.settings.core_settings import CoreSettingsSchema
from core.src.settings.settings import Environment, TrainingSettings, get_settings
from core.src.setup import configure_logging
from core.src.utils.registry.shared_registry import SharedRegistry
from core.src.utils.training_handler import TrainingHandler


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
        if self.app_settings.work_environment.env == Environment.DEVELOPMENT:
            core_settings = CoreSettingsSchema()
            self.registry.put(CoreSettingsSchema, core_settings)
            settings = get_settings()
            self.train(settings.training)
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

            self.inference(handler, settings.training)  # inference until told to stop
            self.train(settings.training)

    @classmethod
    def startup_ray(cls):
        ray.shutdown()
        ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, include_dashboard=True, num_gpus=1)

        if torch.cuda.is_available():
            logger.info(f"GPU is available, using: {torch.cuda.get_device_name(0)}")
        else:
            logger.info("GPU is not available, using cpu")

    def train(self, training_settings: TrainingSettings):
        training_handler = TrainingHandler("PPO", environment_cls=GodotServerEnvironment)
        training_handler.train(training_settings)

    def inference(self, handler: GodotHandler[str], training_settings: TrainingSettings):
        env = GodotServerInferenceEnvironment(connection_handler=handler)
        ModelCatalog.register_custom_model("custom_ppo_model", CustomModel)
        config = PPOConfig().training(model={"custom_model": "custom_ppo_model"})
        policy = PPOTorchPolicy(action_space=env.action_space, observation_space=env.observation_space, config=config)

        obs, _ = env.reset()

        try:
            while True:
                action, _, _ = policy.compute_actions(obs)
                obs, reward, terminateds, truncateds, infos = env.step(action)
        except OSError:
            training_thread = threading.Thread(target=self.train, args=(training_settings,))
            training_thread.start()
