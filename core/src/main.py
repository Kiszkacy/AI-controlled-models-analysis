import os

import ray
import torch
from loguru import logger

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

    training_settings = get_settings().training
    training_manager = TrainingManager(training_settings=training_settings, is_resume=True, save_dir="test_storage")
    training_manager.train()

    # simulation_manager = SimulationManager("try", training_settings)
    # simulation_manager.run(num_episodes=1)
