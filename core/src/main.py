import os

import ray
import torch
from loguru import logger
from ray.rllib.algorithms.ppo.ppo import PPOConfig

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.settings import get_settings
from core.src.setup import configure_logging

if __name__ == "__main__":
    os.environ["RAY_COLOR_PREFIX"] = "1"
    configure_logging()
    ray.init(runtime_env={"worker_process_setup_hook": configure_logging}, configure_logging=False, num_gpus=1)

    DEVICE = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    logger.info(f"Using {DEVICE=}")

    env = GodotServerEnvironment

    training_settings = get_settings().training

    config = (
        PPOConfig()
        .environment(env)
        .rollouts(
            num_rollout_workers=training_settings.number_of_workers,
            create_env_on_local_worker=False,
            num_envs_per_worker=training_settings.number_of_env_per_worker,
        )
        .framework("torch")
        .training(model={"fcnet_hiddens": [64, 64]}, train_batch_size=training_settings.training_batch_size)
    )

    algo = config.build()

    for _ in range(training_settings.training_iterations):
        logger.info(algo.train())

    # To save a model call: algo.save(checkpoint_dir='./model/')
