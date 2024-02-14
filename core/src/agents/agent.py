import abc

from core.src.environments.environment import Environment


class Agent(abc.ABC):
    def __init__(self, environment_cls: type[Environment]):
        self.environment_cls = environment_cls

    @abc.abstractmethod
    def act(self, state): ...
