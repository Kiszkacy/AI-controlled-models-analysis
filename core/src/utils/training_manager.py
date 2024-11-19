import os

from loguru import logger
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import Algorithm, PPOConfig

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.settings import TrainingSettings
from core.src.utils.model_manager import ModelManager


class TrainingManager:
    def __init__(
        self,
        save_dir: str,
        training_settings: TrainingSettings,
        is_resume: bool = False,
        environment_cls: type[MultiAgentEnv] | str = GodotServerEnvironment,
    ):
        self.environment_cls = environment_cls  # Could be hardcoded(?)
        self.training_settings: TrainingSettings = training_settings

        save_path: str = os.path.join(self.training_settings.base_model_dir, save_dir)
        self.model_manager = ModelManager(save_path=save_path)

        self.trainer: Algorithm | None = None
        if is_resume:
            self.trainer = self.load_model()
        else:
            self.trainer = self.build_trainer()

    def build_trainer(self) -> Algorithm:
        model_config = {
            "use_lstm": True,
            "lstm_cell_size": 256,
            "max_seq_len": 32,
            "fcnet_hiddens": [64],
            "lstm_use_prev_action": True,
            "lstm_use_prev_reward": True,
            "_disable_action_flattening": True,
        }

        algorithm_config = (
            PPOConfig()
            .environment(self.environment_cls)
            .rollouts(
                num_rollout_workers=self.training_settings.number_of_workers,
                create_env_on_local_worker=False,
                num_envs_per_worker=self.training_settings.number_of_env_per_worker,
            )
            .resources(
                num_gpus=0,
            )
            .framework("torch")
            .training(
                model=model_config,
                train_batch_size=self.training_settings.training_batch_size,
                lr=1e-4,
                # entropy_coeff=0.01,
                # num_sgd_iter=10,
                # sgd_minibatch_size=128,
                # vf_loss_coeff=2.0,
                # vf_clip_param=15,
                grad_clip=0.5,
                # use_gae=True,
                gamma=0.99,
            )
        )
        self.model_manager.save_config(algorithm_config)
        return algorithm_config.build()

    def train(self):
        for iteration in range(self.training_settings.training_iterations):
            all_info = self.trainer.train()

            sampler_info = all_info["sampler_results"]
            logger.info(
                "episode_reward_mean: {}, episode_reward_max: {}, episode_reward_min: {}, episodes_this_iter: {}".format(  # noqa: E501
                    sampler_info["episode_reward_mean"],
                    sampler_info["episode_reward_max"],
                    sampler_info["episode_reward_min"],
                    sampler_info["episodes_this_iter"],
                )
            )

            if (iteration + 1) % self.training_settings.training_checkpoint_frequency == 0:
                self.model_manager.save_checkpoint(trainer=self.trainer)

    def load_model(self) -> Algorithm | None:
        return self.model_manager.load_model_and_config()
