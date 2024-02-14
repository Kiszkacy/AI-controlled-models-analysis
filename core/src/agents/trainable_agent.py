import abc
from pathlib import Path

from core.src.agents.agent import Agent
from core.src.environments.environment import Environment
from core.src.policies.policy_network import PolicyNetwork


class TrainableAgent(Agent, abc.ABC):
    def __init__(self, policy_network: PolicyNetwork, environment_cls: type[Environment]):
        super().__init__(environment_cls=environment_cls)
        self.policy_network = policy_network

    @abc.abstractmethod
    def load(self, path: Path): ...

    @abc.abstractmethod
    def train(self): ...
