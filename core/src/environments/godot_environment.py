import numpy as np
import json
from gymnasium.spaces import Box, Dict
from ray.rllib.utils.typing import MultiAgentDict

from core.src.settings import get_settings
from core.src.utils.godot_handler import GodotHandler
from ray.rllib.env.multi_agent_env import MultiAgentEnv

environment_settings = get_settings().environment


class GodotServerEnvironment(MultiAgentEnv):
    action_space = Dict({
        'accelerate': Box(
            low=np.array([environment_settings.observation_space_low]),
            high=np.array([environment_settings.observation_space_high]),
            shape=(1,),
            dtype=np.float32
        ),
        'rotate': Box(
            low=np.array([environment_settings.observation_space_low]),
            high=np.array([environment_settings.observation_space_high]),
            shape=(1,),
            dtype=np.float32
        )
    })

    observation_space = Box(
        low=np.array([environment_settings.observation_space_low] * environment_settings.observation_space_size),
        high=np.array([environment_settings.observation_space_high] * environment_settings.observation_space_size),
        shape=(environment_settings.observation_space_size,),
        dtype=np.float32
    )

    ids = [i for i in range(25)]

    def __init__(self, config: dict | None = None):  # noqa: ARG002
        super().__init__()
        print("Initializing GodotServerEnvironment...")
        self._states: MultiAgentDict | None = None
        self.godot_handler = GodotHandler()
        self.godot_handler.launch_godot()

    def step(self, actions: MultiAgentDict):  # noqa: ARG002
        """Returns observations from ready agents.

        The returns are dicts mapping from agent_id strings to values. The
        number of agents in the env can vary over time.

        Returns:
            Tuple containing 1) new observations for
            each ready agent, 2) reward values for each ready agent. If
            the episode is just started, the value will be None.
            3) Terminated values for each ready agent. The special key
            "__all__" (required) is used to indicate env termination.
            4) Truncated values for each ready agent.
            5) Info values for each agent id (maybe empty dicts).
        """
        actions_json = json.dumps(actions)
        self.godot_handler.send(actions_json.encode("utf-8"))
        return self.get_data()

    def get_data(self):
        print("Requesting data...")
        data_list: list[dict] = self.godot_handler.request_data()
        states = {
            data["id"]: np.array([
                data["speed"], data["energy"], data["health"], data["distanceToClosestFood"],
                data["angleToClosestFood"]
            ]) for data in data_list
        }
        rewards = {data["id"]: data["score"] for data in data_list}
        is_done = {data["id"]: False for data in data_list}
        truncated = {data["id"]: False for data in data_list}
        infos = {data["id"]: {} for data in data_list}
        print("Received data:", states)
        return states, rewards, is_done, truncated, infos

    def reset(self, **_kwargs) -> tuple[MultiAgentDict, MultiAgentDict]:
        print("Resetting environment...")
        self._states = self.default_states
        info = {agent_id: {} for agent_id in self._states}
        return self._states, info

    @property
    def states(self) -> MultiAgentDict:
        """Returns current state of the environment."""
        if self._states is None:
            self._states = self.get_data()[0]
        return self._states

    @property
    def default_states(self) -> MultiAgentDict:
        states = {
            agent_id: np.random.default_rng().random(size=environment_settings.observation_space_size)
            for agent_id in self.ids
        }
        print("Generated default states:", states)
        return states
