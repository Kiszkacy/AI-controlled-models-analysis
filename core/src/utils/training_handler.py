import os

from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import Algorithm, PPOConfig
from ray.tune import Tuner

from core.src.agents.agent_policy import AgentPolicy
from core.src.settings import get_settings


class MyAlgo(Algorithm):
    def get_default_policy_class(self, config):  # noqa: ARG002
        return AgentPolicy


def get_path() -> str | None:
    project_root = os.getcwd()
    while os.path.basename(project_root) != "src":
        project_root = os.path.dirname(project_root)
        if not project_root:
            break
    if project_root:
        model_dir = os.path.join(project_root, "model")
        #   import_path = os.path.join(model_dir, "model.pth")
        if not os.path.exists(model_dir):
            os.makedirs(model_dir)
        return model_dir  # import_path
    return None


class TrainingHandler:
    def __init__(  # noqa: PLR0913
        self,
        model_path: str | None = None,
        environment_cls: type[MultiAgentEnv] | str = "Pendulum",
        policy_cls=PPOConfig,
        learning_rate: float = 1e-3,
        gamma: float = 0.99,
    ):
        self.model_path = model_path
        self.environment_cls = environment_cls
        self.policy_cls = policy_cls
        self.learning_rate = learning_rate
        self.gamma = gamma

    def train(self):
        training_settings = get_settings().training
        config = {
            "env": self.environment_cls,
            "framework": "torch",
            "num_workers": training_settings.number_of_workers,
            """   "model": {
                "custom_model": self.policy_cls,
            },"""
            "checkpoint_freq": training_settings.training_checkpoint_frequency,
            "train_batch_size": training_settings.training_batch_size,
            "learning_rate": self.learning_rate,
            "rollout_fragment_length": 100,
            "gamma": self.gamma,
        }
        if self.model_path:
            config["model_path"] = self.model_path
            Tuner(MyAlgo, param_space=config).fit()
        else:
            config["model_path"] = get_path()
            Tuner(MyAlgo, param_space=config).fit()  # Tuner params are saved by default at path ~/ray_results
