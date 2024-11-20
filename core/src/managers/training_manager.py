import os

from loguru import logger
from ray.rllib.algorithms import Algorithm

from core.src.managers.storage_manager import StorageManager
from core.src.managers.trainer_configurator import TrainerConfigurator
from core.src.settings import TrainingSettings


class TrainingManager:
    def __init__(self, save_dir: str, training_settings: TrainingSettings, is_resume: bool = False):
        self.training_settings: TrainingSettings = training_settings
        self.storage_manager: StorageManager = self.create_storage_manager(save_dir)
        self.trainer: Algorithm = self.create_trainer(is_resume)

    def create_storage_manager(self, save_dir: str) -> StorageManager:
        if not save_dir or not isinstance(save_dir, str):
            raise ValueError("save_dir must be a non-empty string.")

        save_path = os.path.join(self.training_settings.base_storage_dir, save_dir)
        return StorageManager(save_path=save_path)

    def create_trainer(self, is_resume: bool) -> Algorithm:
        trainer_configurator = TrainerConfigurator(
            storage_manager=self.storage_manager, training_settings=self.training_settings
        )
        if is_resume:
            return trainer_configurator.load_trainer()
        return trainer_configurator.create_new_trainer()

    def train(self):
        for iteration in range(self.training_settings.training_iterations):
            all_info = self.trainer.train()

            sampler_info = all_info["sampler_results"]
            logger.info(
                "iteration: {}, episode_reward_mean: {}, episode_reward_max: {}, episode_reward_min: {}, episodes_this_iter: {}".format(  # noqa: E501
                    iteration,
                    sampler_info["episode_reward_mean"],
                    sampler_info["episode_reward_max"],
                    sampler_info["episode_reward_min"],
                    sampler_info["episodes_this_iter"],
                )
            )

            if (iteration + 1) % self.training_settings.training_checkpoint_frequency == 0:
                self.storage_manager.save_checkpoint(trainer=self.trainer)
