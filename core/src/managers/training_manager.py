from loguru import logger
from ray.rllib.algorithms import Algorithm
from ray.tune import Tuner

from core.src.managers.algorithm_configurator import AlgorithmConfigurator
from core.src.managers.storage_manager import StorageManager
from core.src.managers.tuner_configurator import TunerConfigurator
from core.src.settings import TrainingSettings, get_settings


class TrainingManager:
    def __init__(self, training_settings: TrainingSettings, storage_manager: StorageManager, tuner: bool):
        self.training_settings: TrainingSettings = training_settings
        self.storage_manager: StorageManager = storage_manager
        self.algorithm: Algorithm | Tuner = self.create_tuner() if tuner else self.create_algorithm()

    def create_algorithm(self) -> Algorithm:
        algorithm_configurator = AlgorithmConfigurator(storage_manager=self.storage_manager)
        if self.training_settings.is_resume:
            return algorithm_configurator.load_algorithm()
        return algorithm_configurator.create_new_algorithm(self.training_settings.config_settings)

    def create_tuner(self) -> Tuner:
        tuner_configurator = TunerConfigurator(
            training_settings=self.training_settings, storage_settings=get_settings().storage
        )
        if self.training_settings.is_resume:
            return tuner_configurator.load_tuner()
        return tuner_configurator.create_new_tuner()

    def train(self):
        if isinstance(self.algorithm, Tuner):
            results = self.algorithm.fit()
            best_result = results.get_best_result(metric="episode_reward_mean", mode="max")
            logger.info(f"Best configuration: {best_result.config}")
        else:
            for iteration in range(self.training_settings.training_iterations):
                all_info = self.algorithm.train()
                sampler_info = all_info["sampler_results"]
                logger.info(
                    "iteration: {}, episode_reward_mean: {}, episode_reward_max: {}, episode_reward_min: {}, episodes_this_iter: {}, timers: {}".format(  # noqa: E501
                        iteration,
                        sampler_info["episode_reward_mean"],
                        sampler_info["episode_reward_max"],
                        sampler_info["episode_reward_min"],
                        sampler_info["episodes_this_iter"],
                        all_info["timers"],
                    )
                )

                if iteration % self.training_settings.training_checkpoint_frequency == 0:
                    self.storage_manager.save_checkpoint(algorithm=self.algorithm, iteration=iteration)
