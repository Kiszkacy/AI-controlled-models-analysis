import os

from loguru import logger
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import PPOConfig

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
        save_interval = training_settings.training_checkpoint_frequency
        model_dir = get_path()

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

        algorithm = ppo_config.build()

        for iteration in range(training_settings.training_iterations):
            all_info = algorithm.train()
            sampler_info = all_info["sampler_results"]
            logger.info(
                "episode_reward_mean: {}, episode_reward_max: {}, episode_reward_min: {}, episodes_this_iter: {}".format(  # noqa: E501
                    sampler_info["episode_reward_mean"],
                    sampler_info["episode_reward_max"],
                    sampler_info["episode_reward_min"],
                    sampler_info["episodes_this_iter"],
                )
            )

            if (iteration + 1) % save_interval == 0:
                checkpoint_path = algorithm.save(model_dir)
                logger.info(f"Model saved at iteration {iteration + 1} to {checkpoint_path}")
