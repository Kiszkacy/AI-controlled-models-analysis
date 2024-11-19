import json
import os

from loguru import logger
from ray.rllib.algorithms import Algorithm


class ModelManager:
    def __init__(self, save_path: str) -> None:
        config_base_path = os.path.join(save_path, "config")
        self.config_path = os.path.join(config_base_path, "config.json")
        self.algorithm_path = os.path.join(config_base_path, "algorithm.json")
        self.checkpoints_path = os.path.join(save_path, "checkpoints")
        os.makedirs(config_base_path, exist_ok=True)
        os.makedirs(self.checkpoints_path, exist_ok=True)

    def save_config(self, config_dict: dict, algorithm: str) -> None:
        try:
            with open(self.config_path, "w") as config_file:
                json.dump(config_dict, config_file, indent=4)
            with open(self.algorithm_path, "w") as algorithm_file:
                algorithm_file.write(algorithm)
            logger.info(f"Saved config to {self.config_path}")
        except Exception as e:
            logger.error(f"Error saving config: {e}")

    def save_checkpoint(self, trainer: Algorithm) -> None:
        try:
            trainer.save_checkpoint(self.checkpoints_path)
            logger.info(f"Model saved to {self.checkpoints_path}")
        except Exception as e:
            logger.error(f"Error saving model: {e}")

    def load_config(self) -> tuple[dict, str]:
        try:
            with open(self.config_path) as config_file:
                config_dict = json.load(config_file)
            logger.info(f"Loaded config from {self.config_path}")

            with open(self.algorithm_path) as algorithm_file:
                algorithm = algorithm_file.read().strip()
            logger.info(f"Loaded algorithm from {self.algorithm_path}")

            return config_dict, algorithm
        except Exception as e:
            logger.error(f"Error loading config: {e}")
            raise

    def load_checkpoint(self, trainer: Algorithm) -> Algorithm:
        try:
            trainer.restore(self.checkpoints_path)
            logger.info(f"Loaded checkpoint from {self.checkpoints_path}")
            return trainer
        except Exception as e:
            logger.error(f"Error loading checkpoint: {e}")
            raise
