from loguru import logger
from ray.rllib.algorithms import Algorithm

from core.src.managers.storage_manager import StorageManager
from core.src.settings import TrainingSettings


class TrainingManager:
    def __init__(self, storage_manager: StorageManager, trainer: Algorithm | None, training_settings: TrainingSettings):
        self.storage_manager = storage_manager
        self.trainer = trainer
        self.training_settings = training_settings

    def train(self):
        for iteration in range(self.training_settings.training_iterations):
            all_info = self.trainer.train()

            sampler_info = all_info["sampler_results"]
            logger.info(
                "episode_reward_mean: {}, episode_reward_max: {}, episode_reward_min: {}, episodes_this_iter: {}".format(  # noqa: E501
                    sampler_info["episode_reward_mean"],
                    sampler_info["episode_reward_max"],
                    sampler_info["episode_reward_min"],
                    sampler_info["episodes_this_iter"],
                )
            )

            if (iteration + 1) % self.training_settings.training_checkpoint_frequency == 0:
                self.storage_manager.save_checkpoint(trainer=self.trainer)
