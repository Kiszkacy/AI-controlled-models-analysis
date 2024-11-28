import json
import os
import shutil

from loguru import logger
from ray.rllib.algorithms import Algorithm


class StorageManager:
    def __init__(self, save_path: str, max_checkpoints: int = 5) -> None:
        config_base_path = os.path.join(save_path, "config")
        self.config_path = os.path.join(config_base_path, "config.json")
        self.algorithm_path = os.path.join(config_base_path, "algorithm.txt")
        self.checkpoints_path = os.path.join(save_path, "checkpoints")
        self.max_checkpoints = max_checkpoints
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

    def save_checkpoint(self, trainer: Algorithm, iteration: int) -> None:
        try:
            checkpoint_path = os.path.join(self.checkpoints_path, f"checkpoint_{iteration}.pth")
            os.makedirs(checkpoint_path, exist_ok=True)
            trainer.save_checkpoint(checkpoint_path)
            logger.info(f"Model saved to {checkpoint_path}")
            self.manage_checkpoints()
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

    def get_latest_checkpoint(self):
        if not os.path.exists(self.checkpoints_path):
            logger.warning("Checkpoint path not found")
            return None

        checkpoints = sorted(
            [f for f in os.listdir(self.checkpoints_path) if f.startswith("checkpoint_")],
            key=lambda x: os.path.getmtime(os.path.join(self.checkpoints_path, x)),
            reverse=True,
        )
        if not checkpoints:
            logger.warning("No checkpoint found")
            return None

        return os.path.join(self.checkpoints_path, checkpoints[0])

    def load_checkpoint(self, trainer: Algorithm, iteration: int | None = None) -> Algorithm:
        try:
            if not iteration:
                checkpoint_path = self.get_latest_checkpoint()
            else:
                checkpoint_path = os.path.join(self.checkpoints_path, f"checkpoint_{iteration}.pth")
            trainer.restore(checkpoint_path)
            logger.info(f"Loaded checkpoint from {checkpoint_path}")
            return trainer
        except Exception as e:
            logger.error(f"Error loading checkpoint: {e}")
            raise

    def manage_checkpoints(self) -> None:
        checkpoints = sorted(
            [f for f in os.listdir(self.checkpoints_path) if f.startswith("checkpoint_")],
            key=lambda x: os.path.getctime(os.path.join(self.checkpoints_path, x)),
        )
        if not checkpoints:
            return
        while len(checkpoints) > self.max_checkpoints:
            try:
                checkpoint_path = os.path.join(self.checkpoints_path, checkpoints.pop(0))
                shutil.rmtree(checkpoint_path)
            except Exception as e:
                logger.error(f"Error deleting file: {e}")
                raise
