import torch
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import Algorithm, DQNConfig, PPOConfig, SACConfig

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.managers.storage_manager import StorageManager
from core.src.settings import TrainingSettings


class TrainerConfigurator:
    def __init__(  # noqa: PLR0913
        self,
        storage_manager: StorageManager,
        training_settings: TrainingSettings,
        algorithm: str = "PPO",
        is_resume: bool = False,
        environment_cls: type[MultiAgentEnv] | str = GodotServerEnvironment,
    ):
        self.environment_cls = environment_cls  # Could be hardcoded(?)
        self.training_settings: TrainingSettings = training_settings
        self.algorithm: str = algorithm
        self.trainer: Algorithm | None = None

        self.storage_manager = storage_manager
        self.set_trainer(is_resume)

    def set_trainer(self, is_resume: bool) -> None:
        if is_resume:
            config_dict, self.algorithm = self.storage_manager.load_config()
            self.trainer = self.build_trainer_from_dict(config_dict)
            self.trainer = self.storage_manager.load_checkpoint(self.trainer)
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
        self.storage_manager.save_config(config_dict=algorithm_config_dict, algorithm=self.algorithm)
        return algorithm_config_dict

    def get_trainer(self) -> Algorithm | None:
        return self.trainer
