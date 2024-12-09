from ray import train, tune
from ray.air import CheckpointConfig
from ray.rllib import MultiAgentEnv
from ray.tune import Tuner
from ray.tune.schedulers import PopulationBasedTraining

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.settings import ConfigSettings


class TunerConfigurator:
    def __init__(
        self,
        config_settings: ConfigSettings,
        environment_cls: type[MultiAgentEnv] | str = GodotServerEnvironment,
    ):
        self.environment_cls = environment_cls
        self.config_settings = config_settings

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
        stopping_criteria = {"training_iteration": 100, "episode_reward_mean": 300}

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
                "num_workers": 4,
                "num_cpus": 1,
                "num_gpus": 0,
                "model": {"free_log_std": True},
                "lambda": 0.95,
                "clip_param": 0.2,
                "lr": 1e-4,
                "num_sgd_iter": tune.choice([10, 20, 30]),
                "sgd_minibatch_size": tune.choice([128, 512, 2048]),
                "train_batch_size": tune.choice([10000, 20000, 40000]),
            },
            run_config=train.RunConfig(
                stop=stopping_criteria,
                checkpoint_config=CheckpointConfig(
                    checkpoint_frequency=1,
                    num_to_keep=3,
                ),
            ),
        )

    @staticmethod
    def load_tuner(path: str, trainable: str) -> Tuner:
        return Tuner.restore(path=path, trainable=trainable)
