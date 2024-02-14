import numpy as np
from gymnasium.spaces import Box, Discrete

from core.src.environments.environment import Environment
from core.src.settings import get_settings
from core.src.utils.godot_handler import GodotHandler


class GymnasiumServerEnvironment(Environment[np.ndarray, np.integer]):
    action_space = Discrete(get_settings().environment.action_space_range)
    # probably should be something more accurate
    observation_space = Box(
        low=-(2**60), high=2**60, shape=(get_settings().environment.observation_space_size,), dtype=np.float32
    )

    def __init__(self, config: dict | None = None):  # noqa: ARG002
        self._state: np.ndarray | None = None
        self.godot_handler = GodotHandler()
        self.godot_handler.launch_godot()

    def step(self, action: np.integer) -> tuple[np.ndarray, float, bool, bool, dict]:
        """
        :param action: Action to be performed in the environment.
        :return: tuple of:
        State of the environment after performing the action,
        Reward for performing the action,
        Whether the game ended or not,
        If truncated,
        Extra info
        """
        self.godot_handler.send(action.tobytes())
        return self.get_data()

    def get_data(self):
        data: dict = self.godot_handler.request_data()
        state = data["state"]
        reward: float = data["reward"]
        is_done: bool = data["is_done"]
        truncated = False
        info = {}
        return state, reward, is_done, truncated, info

    def reset(self, **_kwargs) -> tuple[np.ndarray, dict]:
        """Resets the state of the environment and returns initial observations."""
        return self.default_state, {}

    @property
    def state(self) -> np.ndarray:
        """Returns current state of the environment."""
        if self._state is None:
            self._state = self.get_data()[0]
        return self._state

    @property
    def default_state(self) -> np.ndarray:
        return np.random.default_rng().random(size=get_settings().environment.observation_space_size)
