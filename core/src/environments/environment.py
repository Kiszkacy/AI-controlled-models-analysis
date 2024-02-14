import abc
from typing import Any, TypeVar

import gymnasium as gym

ObsType = TypeVar("ObsType")
ActType = TypeVar("ActType")


class Environment(abc.ABC, gym.Env[ObsType, ActType]):
    @abc.abstractmethod
    def step(self, action: ActType) -> tuple[ObsType, float, bool, bool, dict[str, Any]]:
        """
        :param action: Action to be performed in the environment.
        :return: tuple of:
        State of the environment after performing the action,
        Reward for performing the action,
        Whether the game ended or not
        """

    @abc.abstractmethod
    def reset(self, *, seed: int | None = None, options=None) -> tuple[ObsType, dict[str, Any]]: ...

    @property
    @abc.abstractmethod
    def state(self) -> ObsType: ...
