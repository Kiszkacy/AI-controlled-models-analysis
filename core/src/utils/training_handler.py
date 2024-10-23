import os

from ray import air
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import PPO, PPOConfig
from ray.tune import Tuner

from core.src.settings import get_settings


def get_path() -> str | None:
    project_root = os.getcwd()
    while os.path.basename(project_root) != "src":
        project_root = os.path.dirname(project_root)
        if not project_root:
            break
    if project_root:
        model_dir = os.path.join(project_root, "model")
        import_path = os.path.join(model_dir, "model.pth")
        if not os.path.exists(model_dir):
            os.makedirs(model_dir)
        return import_path
    return None


class TrainingHandler:
    def __init__(
        self,
        model_path: str | None = None,
        environment_cls: type[MultiAgentEnv] | str = "Pendulum",
        learning_rate: float = 1e-3,
        gamma: float = 0.99,
    ):
        self.model_path = model_path
        self.environment_cls = environment_cls
        self.learning_rate = learning_rate
        self.gamma = gamma

    def train(self):
        training_settings = get_settings().training

        ppo_config = PPOConfig().environment(self.environment_cls).framework("torch")

        ppo_config = ppo_config.training(
            gamma=self.gamma,
            lr=self.learning_rate,
            train_batch_size=training_settings.training_batch_size,
        ).rollouts(num_rollout_workers=training_settings.number_of_workers, rollout_fragment_length=100)

        config = ppo_config.to_dict()

        config["checkpoint_freq"] = training_settings.training_checkpoint_frequency

        if self.model_path:
            """ Algorithm can be used in loop, not using it now """
            # print(f"Loading pretrained model from {self.model_path}")
            algorithm = PPO(config=config)
            algorithm.restore(self.model_path)
        else:
            config["model_path"] = get_path()
            algorithm = PPO(config=config)

        tuner = Tuner(
            "PPO",
            param_space=config,
            run_config=air.RunConfig(
                # stop={"timesteps_total": 10000},
                local_dir="~/ray_results",
                checkpoint_config=air.CheckpointConfig(checkpoint_at_end=True, num_to_keep=5),
            ),
        )

        tuner.fit()
