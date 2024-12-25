import os.path

import torch
from ray import train, tune
from ray.air import CheckpointConfig
from ray.rllib import MultiAgentEnv
from ray.tune import Tuner
from ray.tune.schedulers import PopulationBasedTraining

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.settings import ConfigSettings, StorageSettings, TrainingSettings


class TunerConfigurator:
    def __init__(
        self,
        training_settings: TrainingSettings,
        storage_settings: StorageSettings,
        environment_cls: type[MultiAgentEnv] | str = GodotServerEnvironment,
    ):
        self.environment_cls = environment_cls
        self.training_settings = training_settings
        self.config_settings: ConfigSettings = training_settings.config_settings
        self.storage_settings = storage_settings

    def create_new_tuner(self) -> Tuner:
        hyperparam_mutations = {
            "gamma": tune.uniform(0.9, 0.95),
            "lr": tune.choice([1e-4, 5e-5, 2e-5, 1e-5]),
            "entropy_coeff": tune.uniform(0.01, 0.1),
            # "num_sgd_iter": tune.randint(1, 31),
            # "sgd_minibatch_size": tune.randint(32, 256),
            # "train_batch_size": tune.randint(400, 2400),
        }

        pbt = PopulationBasedTraining(
            time_attr="training_iteration",
            perturbation_interval=10,
            resample_probability=0.25,
            hyperparam_mutations=hyperparam_mutations,
            custom_explore_fn=lambda config: config,
        )

        stopping_criteria = {
            "training_iteration": self.training_settings.training_iterations,
            # "episode_reward_mean": 30000,
        }

        return tune.Tuner(
            self.config_settings.algorithm,
            tune_config=tune.TuneConfig(
                metric="episode_reward_mean",
                mode="max",
                scheduler=pbt,
                num_samples=1,
            ),
            param_space={
                "env": self.environment_cls,
                "framework": "torch",
                "num_rollout_workers": self.config_settings.number_of_workers,
                "num_envs_per_worker": self.config_settings.number_of_env_per_worker,
                # "num_cpus": 1,
                "num_gpus": torch.cuda.device_count() if self.config_settings.use_gpu else 0,
                "model": {
                    "use_lstm": True,
                    "lstm_cell_size": self.config_settings.lstm_cell_size,
                    "max_seq_len": self.config_settings.max_seq_len,
                    "fcnet_hiddens": self.config_settings.fcnet_hiddens,
                    "lstm_use_prev_action": True,
                    "lstm_use_prev_reward": True,
                    "_disable_action_flattening": True,
                    # "vf_share_layers": False, (rozdzielenie polityki i funkcji straty)
                    "custom_model_config": {"min_log_std": -10, "max_log_std": 2},
                },
                # "weight_decay": 1e-5,  # normalizacja L2, ograniczenie wag dla modelu
                "gamma": self.config_settings.gamma,
                "clip_param": self.config_settings.clip_param,
                "lr": self.config_settings.lr,  # tune.choice([1e-4, 5e-5, 2e-5, 1e-5]),
                "grad_clip": self.config_settings.grad_clip,
                # "num_sgd_iter": tune.choice([10, 20, 30]),
                # "sgd_minibatch_size": tune.choice([32, 64, 128]),
                "train_batch_size": self.config_settings.training_batch_size,
                # tune.choice([400, 800, 1200, 1600, 2000]),
                "entropy_coeff": self.config_settings.entropy_coeff,
            },
            run_config=train.RunConfig(
                name=self.storage_settings.name,
                storage_path=self.storage_settings.save_path,
                stop=stopping_criteria,
                checkpoint_config=CheckpointConfig(
                    checkpoint_frequency=self.training_settings.training_checkpoint_frequency,
                    num_to_keep=self.storage_settings.max_checkpoints,
                    checkpoint_at_end=True,
                ),
            ),
        )

    def load_tuner(self) -> Tuner:
        path = os.path.join(self.storage_settings.save_path, self.storage_settings.name)
        return Tuner.restore(path=path, trainable=self.config_settings.algorithm)
