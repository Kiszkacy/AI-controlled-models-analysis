import os

import torch
from loguru import logger
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import Algorithm, DQNConfig, PPOConfig, SACConfig

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.settings import TrainingSettings
from core.src.utils.model_manager import ModelManager


class TrainingManager:
    def __init__(  # noqa: PLR0913
        self,
        save_dir: str,
        training_settings: TrainingSettings,
        algorithm: str = "PPO",
        is_resume: bool = False,
        environment_cls: type[MultiAgentEnv] | str = GodotServerEnvironment,
    ):
        self.environment_cls = environment_cls  # Could be hardcoded(?)
        self.training_settings: TrainingSettings = training_settings
        self.algorithm: str = algorithm
        self.trainer: Algorithm | None = None

        self.model_manager = self.create_model_manager(save_dir)
        self.set_trainer(is_resume)

    def set_trainer(self, is_resume: bool) -> None:
        if is_resume:
            config_dict, self.algorithm = self.model_manager.load_config()
            self.trainer = self.build_trainer_from_dict(config_dict)
            self.trainer = self.model_manager.load_checkpoint(self.trainer)
        else:
            config_dict = self.create_config_dict()
            self.trainer = self.build_trainer_from_dict(config_dict)

    def build_trainer_from_dict(self, config_dict: dict) -> Algorithm:
        config_dict.update(
            {
                "env": self.environment_cls,
            }
        )
        if self.algorithm == "PPO":
            return PPOConfig().from_dict(config_dict).build()

        if self.algorithm == "DQN":
            return DQNConfig().from_dict(config_dict).build()

        if self.algorithm == "SAC":
            return SACConfig().from_dict(config_dict).build()
        raise ValueError(f"Unsupported algorithm: {self.algorithm}")

    def create_config_dict(self) -> dict:
        algorithm_config_dict = {
            "rollouts": {
                "num_rollout_workers": self.training_settings.number_of_workers,
                "create_env_on_local_worker": False,
                "num_envs_per_worker": self.training_settings.number_of_env_per_worker,
            },
            "resources": {
                "num_gpus": torch.cuda.device_count() if self.training_settings.use_gpu else 0,
            },
            "framework": "torch",
            "training": {
                "model": {
                    "use_lstm": True,
                    "lstm_cell_size": 256,
                    "max_seq_len": 32,
                    "fcnet_hiddens": [64],
                    "lstm_use_prev_action": True,
                    "lstm_use_prev_reward": True,
                    "_disable_action_flattening": True,
                },
                "train_batch_size": self.training_settings.training_batch_size,
                "lr": 1e-4,
                "grad_clip": 0.5,
                "gamma": 0.99,
                # "entropy_coeff": 0.01,
                # "num_sgd_iter": 10,
                # "sgd_minibatch_size": 128,
                # "vf_loss_coeff": 2.0,
                # "vf_clip_param": 15,
                # "use_gae": True,
            },
        }
        self.model_manager.save_config(config_dict=algorithm_config_dict, algorithm=self.algorithm)
        return algorithm_config_dict

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
                self.model_manager.save_checkpoint(trainer=self.trainer)

    def create_model_manager(self, save_dir):
        if not save_dir or not isinstance(save_dir, str):
            raise ValueError("save_dir must be a non-empty string.")

        save_path: str = os.path.join(self.training_settings.base_model_dir, save_dir)
        return ModelManager(save_path)
