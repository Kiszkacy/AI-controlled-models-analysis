import torch
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import Algorithm, DQNConfig, PPOConfig, SACConfig

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.managers.storage_manager import StorageManager
from core.src.settings import TrainingSettings


class AlgorithmConfigurator:
    def __init__(
        self,
        storage_manager: StorageManager,
        training_settings: TrainingSettings,
        environment_cls: type[MultiAgentEnv] | str = GodotServerEnvironment,
    ):
        self.environment_cls = environment_cls
        self.training_settings: TrainingSettings = training_settings
        self.storage_manager = storage_manager

    def build_trainer_from_dict(self, config_dict: dict, algorithm_cls: str) -> Algorithm:
        config_dict.update(
            {
                "env": self.environment_cls,
            }
        )
        if algorithm_cls == "PPO":
            return PPOConfig().from_dict(config_dict).build()

        if algorithm_cls == "DQN":
            return DQNConfig().from_dict(config_dict).build()

        if algorithm_cls == "SAC":
            return SACConfig().from_dict(config_dict).build()
        raise ValueError(f"Unsupported algorithm: {algorithm_cls}")

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
                    "fcnet_hiddens": [512, 256],
                    "lstm_use_prev_action": True,
                    "lstm_use_prev_reward": True,
                    "_disable_action_flattening": True,
                    # "vf_share_layers": False, (rozdzielenie polityki i funkcji straty)
                },
                "train_batch_size": self.training_settings.training_batch_size,
                "lr": 1e-4,
                "grad_clip": 0.1,
                "gamma": 0.95,
                "entropy_coeff": 0.01,
                "clip_param": 0.1,
                # "num_sgd_iter": 10,
                # "sgd_minibatch_size": 128,
                # "vf_loss_coeff": 1.0,
                # "vf_clip_param": 15,
                # "use_gae": True,
            },
        }
        algorithm_cls = self.training_settings.algorithm
        self.storage_manager.save_config(config_dict=algorithm_config_dict, algorithm_cls=algorithm_cls)
        return algorithm_config_dict, algorithm_cls

    def load_algorithm(self) -> Algorithm:
        config_dict, algorithm_cls = self.storage_manager.load_config()
        algorithm = self.build_trainer_from_dict(config_dict, algorithm_cls)
        return self.storage_manager.load_checkpoint(algorithm)

    def create_new_algorithm(self) -> Algorithm:
        config_dict, algorithm_cls = self.create_config_dict()
        return self.build_trainer_from_dict(config_dict, algorithm_cls)
