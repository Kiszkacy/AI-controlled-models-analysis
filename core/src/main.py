import os

import ray
import torch
from loguru import logger

from core.src.managers.storage_manager import StorageManager
from core.src.managers.trainer_configurator import TrainerConfigurator
from core.src.managers.training_manager import TrainingManager
from core.src.settings import get_settings
from core.src.setup import configure_logging

if __name__ == "__main__":
    os.environ["RAY_COLOR_PREFIX"] = "1"

    ray.shutdown()
    ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, configure_logging=False)

    DEVICE = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    logger.info(f"Using {DEVICE=}")

    save_dir = "try"
    training_settings = get_settings().training
    if not save_dir or not isinstance(save_dir, str):
        raise ValueError("save_dir must be a non-empty string.")

    save_path = os.path.join(training_settings.base_storage_dir, save_dir)
    storage_manager = StorageManager(save_path=save_path)

    trainer_configurator = TrainerConfigurator(
        training_settings=training_settings, is_resume=True, storage_manager=storage_manager
    )

    training_manager = TrainingManager(
        training_settings=training_settings, storage_manager=storage_manager, trainer=trainer_configurator.get_trainer()
    )

    training_manager.train()
