import json

import numpy as np
from gymnasium.spaces import Box, Dict
from ray.rllib.env.multi_agent_env import MultiAgentEnv
from ray.rllib.utils.typing import MultiAgentDict

from core.src.settings import get_settings
from core.src.utils.godot_handler import GodotHandler

environment_settings = get_settings().environment


class GodotServerEnvironment(MultiAgentEnv):
    action_space = Dict(
        {
            "accelerate": Box(
                low=environment_settings.observation_space_low,
                high=environment_settings.observation_space_high,
                shape=(),
                dtype=np.float32,
            ),
            "rotate": Box(
                low=environment_settings.observation_space_low,
                high=environment_settings.observation_space_high,
                shape=(),
                dtype=np.float32,
            ),
        }
    )

    observation_space = Box(
        low=-(2**60),
        # needs to be changed to observation_space_low/high when it's fixed in godot
        high=2**60,
        shape=(environment_settings.observation_space_size,),
        dtype=np.float32,
    )

    def __init__(self, config: dict | None = None):  # noqa: ARG002
        super().__init__()
        self._agent_ids = set(range(environment_settings.number_of_agents))
        self._states: MultiAgentDict | None = None
        self.godot_handler = GodotHandler()
        self.godot_handler.launch_godot()

    def step(self, actions: MultiAgentDict):
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
        actions_serializable = [
            {"id": key, "accelerate": value["accelerate"], "rotate": value["rotate"]} for key, value in actions.items()
        ]

        actions_json = json.dumps(actions_serializable)
        self.godot_handler.send(actions_json.encode("utf-8"))
        return self.get_data()

    def get_data(self):
        try:
            data_list = self.godot_handler.request_data()
        except json.JSONDecodeError:
            raise

        states = {
            data["Id"]: np.array(
                [
                    data["Speed"],
                    data["Energy"],
                    data["Health"],
                    data["DistanceToClosestFood"],
                    data["AngleToClosestFood"],
                ]
            )
            for data in data_list
        }
        rewards = {data["Id"]: data["Score"] for data in data_list}
        terminateds = {"__all__": False}
        truncateds = {"__all__": False}
        infos = {data["Id"]: {} for data in data_list}
        return states, rewards, terminateds, truncateds, infos

    def reset(self, *, seed=None, options=None) -> tuple[MultiAgentDict, MultiAgentDict]:  # noqa: ARG002
        self._states = self.default_states
        info: MultiAgentDict = {"__all__": {}}
        return self._states, info

    @property
    def states(self) -> MultiAgentDict:
        """Returns current state of the environment."""
        if self._states is None:
            self._states = self.get_data()[0]
        return self._states

    @property
    def default_states(self) -> MultiAgentDict:
        return {
            agent_id: np.random.default_rng().random(size=environment_settings.observation_space_size)
            for agent_id in self._agent_ids
        }
