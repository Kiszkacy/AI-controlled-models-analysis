import json

import numpy as np
from gymnasium.spaces import Box, Dict
from ray.rllib.env.multi_agent_env import MultiAgentEnv
from ray.rllib.utils.typing import MultiAgentDict

from core.src.settings.settings import AgentEnvironmentSettings, get_settings

__all__ = ["GodotServerInferenceEnvironment"]


class GodotServerInferenceEnvironment(MultiAgentEnv):
    def __init__(self, config: dict | None = None, connection_handler=None):  # noqa: ARG002
        super().__init__()

        environment_settings = get_settings().environment
        self.action_space = self.get_action_space(environment_settings)
        self.observation_space = self.get_observation_space(environment_settings)

        self._agent_ids = set(range(environment_settings.number_of_agents))
        self._states: MultiAgentDict | None = None
        self.communication_settings = get_settings().communication_codes

        self.connection_handler = connection_handler

    @staticmethod
    def get_action_space(environment_settings: AgentEnvironmentSettings) -> Dict:
        return Dict(
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

    @staticmethod
    def get_observation_space(environment_settings: AgentEnvironmentSettings) -> Box:
        return Box(
            low=environment_settings.observation_space_low,
            high=environment_settings.observation_space_high,
            shape=(environment_settings.observation_space_size,),
            dtype=np.float32,
        )

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
        actions["accelerate"] = actions["accelerate"].astype(float)
        actions["rotate"] = actions["rotate"].astype(float)
        actions_serializable = [
            {"id": key, "accelerate": acc, "rotate": rot}
            for key, (acc, rot) in enumerate(zip(*actions.values(), strict=False))
        ]

        actions_json = json.dumps(actions_serializable)
        self.connection_handler.send(actions_json.encode("utf-8"))
        return self.get_data()

    def get_data(self):
        received_data = self.connection_handler.receive()

        if isinstance(received_data, int):
            raise OSError

        received_data = json.loads(received_data)

        # GODOT Schema: [id, reward, terminated, observations...]
        id_idx, reward_idx, terminated_idx, observations_idx = range(4)

        observations = []
        rewards = []
        terminateds = {"__all__": False}
        truncateds = {"__all__": False}
        infos = {}

        for data in received_data:
            agent_id = data[id_idx]
            rewards.append(data[reward_idx])
            terminateds[agent_id] = data[terminated_idx]

            if not data[terminated_idx]:
                observations.append(np.array(data[observations_idx:]))
                infos[agent_id] = {}

        return np.array(observations), np.array(rewards), terminateds, truncateds, infos

    def reset(self, *, seed=None, options=None) -> tuple[MultiAgentDict, MultiAgentDict]:  # noqa: ARG002
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
