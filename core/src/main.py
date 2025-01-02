import os

import ray
import torch
from loguru import logger

from core.src.managers.simulation_manager import SimulationManager
from core.src.managers.storage_manager import StorageManager
from core.src.managers.training_manager import TrainingManager
from core.src.settings import get_settings
from core.src.setup import configure_logging

if __name__ == "__main__":
    os.environ["RAY_COLOR_PREFIX"] = "1"

    ray.shutdown()
    ray.init(
        runtime_env={"worker_process_setup_hook": configure_logging}, configure_logging=False, include_dashboard=True
    )

    DEVICE = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    logger.info(f"Using {DEVICE=}")

    training = True

    storage_settings = get_settings().storage
    storage_manager = StorageManager(storage_settings)

    if training:
        training_settings = get_settings().training
        training_manager = TrainingManager(
            training_settings=training_settings, storage_manager=storage_manager, tuner=False
        )
        training_manager.train()
    else:
        simulation_manager = SimulationManager(storage_manager=storage_manager)
        simulation_manager.run(num_episodes=1)
