import json
import os

from loguru import logger
from ray.rllib.algorithms import Algorithm, AlgorithmConfig


class ModelManager:
    def __init__(self, save_path: str) -> None:
        config_base_path = os.path.join(save_path, "config")
        self.config_path = os.path.join(config_base_path, "config.json")
        self.checkpoints_path = os.path.join(save_path, "checkpoints")
        os.makedirs(config_base_path, exist_ok=True)
        os.makedirs(self.checkpoints_path, exist_ok=True)

    def save_config(self, config: AlgorithmConfig) -> None:
        config_dict = {key: value for key, value in config.items() if key != "env"}
        try:
            with open(self.config_path, "w") as config_file:
                json.dump(config_dict, config_file)
            logger.info(f"Saved config to {self.config_path}")
        except Exception as e:
            logger.error(f"Error saving config: {e}")

    def save_checkpoint(self, trainer: Algorithm) -> None:
        try:
            trainer.save_checkpoint(self.checkpoints_path)
            logger.info(f"Model saved to {self.checkpoints_path}")
        except Exception as e:
            logger.error(f"Error saving model: {e}")

    def load_config(self) -> Algorithm | None:
        try:
            with open(self.config_path) as config_file:
                config_dict = json.load(config_file)
            config = AlgorithmConfig.from_dict(config_dict)
            logger.info(f"Loaded config from {self.config_path}")
            return config.build()
        except Exception as e:
            logger.error(f"Error loading config: {e}")
            return None

    def load_checkpoint(self, trainer: Algorithm) -> Algorithm | None:
        try:
            trainer.restore(self.checkpoints_path)
            logger.info(f"Loaded checkpoint from {self.checkpoints_path}")
            return trainer
        except Exception as e:
            logger.error(f"Error loading checkpoint: {e}")
            return None

    def load_model_and_config(self) -> Algorithm | None:
        trainer = self.load_config()
        if trainer is None:
            logger.error("Failed to load config, cannot load checkpoint.")
            return None
        return self.load_checkpoint(trainer)