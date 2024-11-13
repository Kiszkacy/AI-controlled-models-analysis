import os

from loguru import logger
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import PPOConfig

from core.src.environments.godot_environment import GodotServerEnvironment
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
        environment_cls: type[MultiAgentEnv] | str = GodotServerEnvironment,
    ):
        self.model_name = model_name
        self.environment_cls = environment_cls

    def train(self):
        training_settings = get_settings().training
        save_interval = training_settings.training_checkpoint_frequency
        model_dir = get_path()

        model_config = {
            "use_lstm": True,
            "lstm_cell_size": 256,
            "max_seq_len": 32,
            "fcnet_hiddens": [128, 64],
            "lstm_use_prev_action": True,
            "lstm_use_prev_reward": True,
            "_disable_action_flattening": True,
        }

        ppo_config = (
            PPOConfig()
            .environment(self.environment_cls)
            .rollouts(
                num_rollout_workers=training_settings.number_of_workers,
                create_env_on_local_worker=False,
                num_envs_per_worker=training_settings.number_of_env_per_worker,
            )
            .resources(
                num_gpus=0,
            )
            .framework("torch")
            .training(
                model=model_config,
                train_batch_size=training_settings.training_batch_size,
                lr=1e-4,
                entropy_coeff=0.01,
                num_sgd_iter=10,
                sgd_minibatch_size=1024,
                vf_clip_param=15,
                # grad_clip=10.0,
                use_gae=True,
                gamma=0.99,
            )
        )

        trainer = ppo_config.build()

        if self.model_name:
            model_dir = get_path() + "\\" + self.model_name
            if os.path.exists(model_dir):
                trainer.restore(model_dir)

        for iteration in range(training_settings.training_iterations):
            all_info = trainer.train()

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
                checkpoint_path = trainer.save(model_dir)
                logger.info(f"Model saved at iteration {iteration + 1} to {checkpoint_path}")
