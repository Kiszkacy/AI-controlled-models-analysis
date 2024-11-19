import os

import ray
import torch
from loguru import logger

from core.src.settings import get_settings
from core.src.setup import configure_logging
from core.src.utils.training_manager import TrainingManager

if __name__ == "__main__":
    os.environ["RAY_COLOR_PREFIX"] = "1"

    ray.shutdown()
    ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, configure_logging=False)

    DEVICE = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    logger.info(f"Using {DEVICE=}")

    settings = get_settings()
    training_handler = TrainingManager(save_dir="try", training_settings=settings.training, is_resume=True)
    training_handler.train()
