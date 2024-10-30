import os

from ray import air
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import PPOConfig
from ray.tune import Tuner

from core.src.settings import get_settings


def get_path() -> str | None:
    project_root = os.getcwd()
    while os.path.basename(project_root) != "src":
        project_root = os.path.dirname(project_root)
        if not project_root:
            break
    if project_root:
        return os.path.join(project_root, "model")
    return None


class TrainingHandler:
    def __init__(
        self,
        model_name: str | None = None,
        environment_cls: type[MultiAgentEnv] | str = "Pendulum",
        learning_rate: float = 1e-3,
        gamma: float = 0.99,
    ):
        self.model_name = model_name
        self.environment_cls = environment_cls
        self.learning_rate = learning_rate
        self.gamma = gamma

    def train(self):
        training_settings = get_settings().training

        ppo_config = (
            PPOConfig()
            .environment(self.environment_cls)
            .rollouts(
                num_rollout_workers=training_settings.number_of_workers,
                create_env_on_local_worker=False,
                num_envs_per_worker=training_settings.number_of_env_per_worker,
                rollout_fragment_length="auto",
            )
            .resources(
                num_learner_workers=1,
                num_gpus=0,
            )
            .framework("torch")
            .training(
                model={"fcnet_hiddens": [128, 128, 128]},
                train_batch_size=training_settings.training_batch_size,
                lr=1e-4,
                entropy_coeff=0.01,
                num_sgd_iter=50,
                sgd_minibatch_size=256,
                vf_clip_param=1,
                grad_clip=40.0,
                use_gae=True,
            )
        )
        config = ppo_config.to_dict()

        config["checkpoint_freq"] = training_settings.training_checkpoint_frequency

        if self.model_name:
            """ Restoring trained model stored in model dir by passed model_name """
            tuner = Tuner.restore(get_path() + "/" + self.model_name, trainable="PPO")

        else:
            tuner = Tuner(
                "PPO",
                param_space=config,
                run_config=air.RunConfig(
                    # stop={"timesteps_total": training_settings.training_iterations},
                    checkpoint_config=air.CheckpointConfig(checkpoint_at_end=True, num_to_keep=5),
                    storage_path=get_path(),
                ),
            )

        tuner.fit()
