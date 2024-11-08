import json

import numpy as np
from gymnasium.spaces import Box, Dict
from ray.rllib.env.multi_agent_env import MultiAgentEnv
from ray.rllib.utils.typing import MultiAgentDict

from core.src.communication.environment.godot_environment_handler import create_godot_environment
from core.src.settings import get_settings

__all__ = ["GodotServerEnvironment"]

environment_settings = get_settings().environment
communication_settings = get_settings().communication
godot_settings = get_settings().godot


class GodotServerEnvironment(MultiAgentEnv):
    action_space = Dict(
        {
            "accelerate": Box(
                low=environment_settings.action_space_low,
                high=environment_settings.action_space_high,
                shape=(),
                dtype=np.float32,
            ),
            "rotate": Box(
                low=environment_settings.action_space_low,
                high=environment_settings.action_space_high,
                shape=(),
                dtype=np.float32,
            ),
        }
    )

    observation_space = Box(
        low=environment_settings.observation_space_low,
        high=environment_settings.observation_space_high,
        shape=(environment_settings.observation_space_size,),
        dtype=np.float32,
    )

    def __init__(self, config: dict | None = None):  # noqa: ARG002
        super().__init__()
        self._agent_ids = set(range(environment_settings.number_of_agents))
        self._states: MultiAgentDict | None = None
        self.connection_handler = create_godot_environment(godot_settings)
        self.connection_handler.acquire_resources()

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
        self.connection_handler.send(actions_json.encode("utf-8"))
        return self.get_data()

    def get_data(self):
        try:
            received_data = self.connection_handler.receive()
        except json.JSONDecodeError:
            raise

        if not isinstance(received_data, list):
            observations, rewards, _, truncateds, infos = self.get_data()
            terminateds = {"__all__": True}
            return observations, rewards, terminateds, truncateds, infos

        # GODOT Schema: [id, reward, terminated, observations...]
        id_idx, reward_idx, terminated_idx, observations_idx = range(4)

        observations = {}
        rewards = {}
        terminateds = {"__all__": False}
        truncateds = {"__all__": False}
        infos = {}

        for data in received_data:
            agent_id = data[id_idx]
            rewards[agent_id] = data[reward_idx]
            terminateds[agent_id] = data[terminated_idx]

            if not data[terminated_idx]:
                observations[agent_id] = np.array(data[observations_idx:])
                infos[agent_id] = {}

        return observations, rewards, terminateds, truncateds, infos

    def reset(self, *, seed=None, options=None) -> tuple[MultiAgentDict, MultiAgentDict]:  # noqa: ARG002
        reset_signal = communication_settings.reset
        byte_message = str(reset_signal).encode()
        self.connection_handler.send(byte_message)

        self._states = None
        observations = self.states[0]
        infos = self.states[-1]
        return observations, infos

    @property
    def states(self) -> MultiAgentDict:
        """Returns current state of the environment."""
        if self._states is None:
            self._states = self.get_data()
        return self._states

    def close(self) -> None:
        self.connection_handler.release_resources()
