import os.path

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
        def explore(config):
            if config["train_batch_size"] < config["sgd_minibatch_size"] * 2:
                config["train_batch_size"] = config["sgd_minibatch_size"] * 2
            if config["num_sgd_iter"] < 1:
                config["num_sgd_iter"] = 1
            return config

        hyperparam_mutations = {
            "lambda": tune.uniform(0.9, 1.0),
            "clip_param": tune.uniform(0.01, 0.5),
            "lr": tune.choice([1e-3, 5e-4, 1e-4, 5e-5, 1e-5]),
            "num_sgd_iter": tune.randint(1, 31),
            "sgd_minibatch_size": tune.randint(128, 16384),
            "train_batch_size": tune.randint(2000, 160000),
        }

        pbt = PopulationBasedTraining(
            time_attr="time_total_s",
            perturbation_interval=120,
            resample_probability=0.25,
            hyperparam_mutations=hyperparam_mutations,
            custom_explore_fn=explore,
        )
        stopping_criteria = {
            "training_iteration": self.training_settings.training_iterations,
            "episode_reward_mean": 30000,
        }

        return tune.Tuner(
            "PPO",
            tune_config=tune.TuneConfig(
                metric="episode_reward_mean",
                mode="max",
                scheduler=pbt,
                num_samples=1,
            ),
            param_space={
                "env": self.environment_cls,
                "kl_coeff": 1.0,
                "num_workers": self.config_settings.number_of_workers,
                "num_cpus": 1,
                "num_gpus": 0,
                "model": {
                    "use_lstm": True,
                    "lstm_cell_size": self.config_settings.lstm_cell_size,
                    "max_seq_len": self.config_settings.max_seq_len,
                    "fcnet_hiddens": self.config_settings.fcnet_hiddens,
                    "lstm_use_prev_action": True,
                    "lstm_use_prev_reward": True,
                    "_disable_action_flattening": True,
                    # "vf_share_layers": False, (rozdzielenie polityki i funkcji straty)
                },
                "gamma": self.config_settings.gamma,
                "clip_param": self.config_settings.clip_param,
                "lr": self.config_settings.lr,
                "num_sgd_iter": tune.choice([10, 20, 30]),
                "sgd_minibatch_size": tune.choice([128, 512, 2048]),
                "train_batch_size": tune.choice([10000, 20000, 40000]),
            },
            run_config=train.RunConfig(
                name=self.storage_settings.name,
                storage_path=self.storage_settings.save_path,
                stop=stopping_criteria,
                checkpoint_config=CheckpointConfig(
                    checkpoint_frequency=self.training_settings.training_checkpoint_frequency,
                    num_to_keep=self.storage_settings.max_checkpoints,
                ),
            ),
        )

    def load_tuner(self) -> Tuner:
        path = os.path.join(self.storage_settings.save_path, self.storage_settings.name)
        return Tuner.restore(path=path, trainable=self.config_settings.algorithm)
