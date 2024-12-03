import json
import os
import shutil

from loguru import logger
from ray.rllib.algorithms import Algorithm

from core.src.settings import StorageSettings


class StorageManager:
    def __init__(self, storage_settings: StorageSettings) -> None:
        config_base_path = os.path.join(storage_settings.save_path, "config")
        self.config_path = os.path.join(config_base_path, "config.json")
        self.algorithm_cls_path = os.path.join(config_base_path, "algorithm.txt")
        self.checkpoints_path = os.path.join(storage_settings.save_path, "checkpoints")
        self.storage_settings = storage_settings
        os.makedirs(config_base_path, exist_ok=True)
        os.makedirs(self.checkpoints_path, exist_ok=True)

    def save_config(self, config_dict: dict, algorithm_cls: str) -> None:
        try:
            with open(self.config_path, "w") as config_file:
                json.dump(config_dict, config_file, indent=4)
            with open(self.algorithm_cls_path, "w") as algorithm_file:
                algorithm_file.write(algorithm_cls)
            logger.info(f"Saved config to {self.config_path}")
        except Exception as e:
            logger.error(f"Error saving config: {e}")

    def save_checkpoint(self, algorithm: Algorithm, iteration: int) -> None:
        try:
            checkpoint_path = os.path.join(self.checkpoints_path, f"checkpoint_{iteration}")
            os.makedirs(checkpoint_path, exist_ok=True)
            algorithm.save_checkpoint(checkpoint_path)
            logger.info(f"Model saved to {checkpoint_path}")
            self.manage_checkpoints()
        except Exception as e:
            logger.error(f"Error saving model: {e}")

    def load_config(self) -> tuple[dict, str]:
        try:
            with open(self.config_path) as config_file:
                config_dict = json.load(config_file)
            logger.info(f"Loaded config from {self.config_path}")

            with open(self.algorithm_cls_path) as algorithm_file:
                algorithm_cls = algorithm_file.read().strip()
            logger.info(f"Loaded algorithm from {self.algorithm_cls_path}")

            return config_dict, algorithm_cls
        except Exception as e:
            logger.error(f"Error loading config: {e}")
            raise

    def get_latest_checkpoint(self) -> str | None:
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

    def load_checkpoint(self, algorithm: Algorithm) -> Algorithm:
        try:
            if not self.storage_settings.restore_iteration:
                checkpoint_path = self.get_latest_checkpoint()
            else:
                checkpoint_path = os.path.join(
                    self.checkpoints_path, f"checkpoint_{self.storage_settings.restore_iteration}"
                )
            algorithm.restore(checkpoint_path)
            logger.info(f"Loaded checkpoint from {checkpoint_path}")
            return algorithm
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
        while len(checkpoints) > self.storage_settings.max_checkpoints:
            try:
                checkpoint_path = os.path.join(self.checkpoints_path, checkpoints.pop(0))
                shutil.rmtree(checkpoint_path)
            except Exception as e:
                logger.error(f"Error deleting file: {e}")
                raise
