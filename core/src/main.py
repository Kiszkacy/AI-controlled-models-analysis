import os

import ray
import torch
from loguru import logger

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.setup import configure_logging
from core.src.utils.training_handler import TrainingHandler

if __name__ == "__main__":
    os.environ["RAY_COLOR_PREFIX"] = "1"

    ray.shutdown()
    ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, include_dashboard=True, num_gpus=1)

    if torch.cuda.is_available():
        logger.info(f"GPU is available, using: {torch.cuda.get_device_name(0)}")
    else:
        logger.info("GPU is not available, using cpu")

    training_handler = TrainingHandler("PPO", environment_cls=GodotServerEnvironment)
    training_handler.train()
