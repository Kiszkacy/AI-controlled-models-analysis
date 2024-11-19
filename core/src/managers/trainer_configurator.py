import torch
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import Algorithm, DQNConfig, PPOConfig, SACConfig

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.managers.storage_manager import StorageManager
from core.src.settings import TrainingSettings


class TrainerConfigurator:
    def __init__(
        self,
        storage_manager: StorageManager,
        training_settings: TrainingSettings,
        environment_cls: type[MultiAgentEnv] | str = GodotServerEnvironment,
    ):
        self.environment_cls = environment_cls
        self.training_settings: TrainingSettings = training_settings
        self.storage_manager = storage_manager

    def build_trainer_from_dict(self, config_dict: dict, algorithm: str) -> Algorithm:
        config_dict.update(
            {
                "env": self.environment_cls,
            }
        )
        if algorithm == "PPO":
            return PPOConfig().from_dict(config_dict).build()

        if algorithm == "DQN":
            return DQNConfig().from_dict(config_dict).build()

        if algorithm == "SAC":
            return SACConfig().from_dict(config_dict).build()
        raise ValueError(f"Unsupported algorithm: {algorithm}")

    def create_config_dict(self) -> tuple[dict, str]:
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
        algorithm = self.training_settings.algorithm
        self.storage_manager.save_config(config_dict=algorithm_config_dict, algorithm=algorithm)
        return algorithm_config_dict, algorithm

    def load_trainer(self) -> Algorithm:
        config_dict, algorithm = self.storage_manager.load_config()
        trainer = self.build_trainer_from_dict(config_dict, algorithm)
        return self.storage_manager.load_checkpoint(trainer)

    def create_new_trainer(self) -> Algorithm:
        config_dict, algorithm = self.create_config_dict()
        return self.build_trainer_from_dict(config_dict, algorithm)
