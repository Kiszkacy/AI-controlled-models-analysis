import torch
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import Algorithm, DQNConfig, PPOConfig, SACConfig

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.managers.storage_manager import StorageManager
from core.src.settings import ConfigSettings


class AlgorithmConfigurator:
    def __init__(
        self,
        storage_manager: StorageManager,
        environment_cls: type[MultiAgentEnv] | str = GodotServerEnvironment,
    ):
        self.environment_cls = environment_cls
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

    def create_config_dict(self, config_settings: ConfigSettings) -> tuple[dict, str]:
        algorithm_config_dict = {
            "rollouts": {
                "num_rollout_workers": config_settings.number_of_workers,
                "create_env_on_local_worker": False,
                "num_envs_per_worker": config_settings.number_of_env_per_worker,
            },
            "resources": {
                "num_gpus": torch.cuda.device_count() if config_settings.use_gpu else 0,
            },
            "framework": "torch",
            "training": {
                "model": {
                    "use_lstm": True,
                    "lstm_cell_size": config_settings.lstm_cell_size,
                    "max_seq_len": config_settings.max_seq_len,
                    "fcnet_hiddens": config_settings.fcnet_hiddens,
                    "lstm_use_prev_action": True,
                    "lstm_use_prev_reward": True,
                    "_disable_action_flattening": True,
                    # "vf_share_layers": False, (rozdzielenie polityki i funkcji straty)
                },
                "train_batch_size": config_settings.training_batch_size,
                "lr": config_settings.lr,
                "grad_clip": config_settings.grad_clip,
                "gamma": config_settings.gamma,
                "entropy_coeff": config_settings.entropy_coeff,
                "clip_param": config_settings.clip_param,
                # "num_sgd_iter": 10,
                # "sgd_minibatch_size": 128,
                # "vf_loss_coeff": 1.0,
                # "vf_clip_param": 15,
                # "use_gae": True,
            },
        }
        algorithm_cls = config_settings.algorithm
        self.storage_manager.save_config(config_dict=algorithm_config_dict, algorithm_cls=algorithm_cls)
        return algorithm_config_dict, algorithm_cls

    def load_algorithm(self) -> Algorithm:
        config_dict, algorithm_cls = self.storage_manager.load_config()
        algorithm = self.build_trainer_from_dict(config_dict, algorithm_cls)
        return self.storage_manager.load_checkpoint(algorithm)

    def create_new_algorithm(self, config_settings: ConfigSettings) -> Algorithm:
        config_dict, algorithm_cls = self.create_config_dict(config_settings)
        return self.build_trainer_from_dict(config_dict, algorithm_cls)
