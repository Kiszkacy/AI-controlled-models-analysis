from loguru import logger
from ray.rllib import MultiAgentEnv

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.managers.algorithm_configurator import AlgorithmConfigurator
from core.src.managers.storage_manager import StorageManager


class SimulationManager:
    def __init__(
        self,
        storage_manager: StorageManager,
        environment_cls: type[MultiAgentEnv] = GodotServerEnvironment,
    ):
        self.env = environment_cls()
        self.algorithm = AlgorithmConfigurator(storage_manager=storage_manager).load_algorithm()

    def run(self, num_episodes: int = 10) -> None:
        for episode in range(num_episodes):
            obs, _ = self.env.reset()
            done = False
            episode_reward = 0

            while not done:
                actions = {}
                for agent_id, agent_obs in obs.items():
                    action = self.algorithm.compute_single_action(agent_obs)
                    actions[agent_id] = action

                next_obs, rewards, dones, _, _ = self.env.step(actions)

                episode_reward += sum(rewards.values())

                obs = next_obs
                done = all(dones.values())

            logger.info(f"Episode {episode + 1}/{num_episodes} finished with reward {episode_reward}")
