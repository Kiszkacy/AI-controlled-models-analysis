import os

from loguru import logger
from ray.rllib import MultiAgentEnv
from ray.rllib.algorithms import Algorithm

from core.src.environments.godot_environment import GodotServerEnvironment
from core.src.managers.algorithm_configurator import AlgorithmConfigurator
from core.src.managers.storage_manager import StorageManager
from core.src.settings import TrainingSettings


class SimulationManager:
    def __init__(
        self,
        training_settings: TrainingSettings,
        environment_cls: type[MultiAgentEnv] = GodotServerEnvironment,
    ):
        self.training_settings = training_settings
        self.storage_manager = self.create_storage_manager(training_settings.save_dir)
        self.env = environment_cls()
        self.algorithm = self.get_algorithm(training_settings.restore_iteration)

    def create_storage_manager(self, save_dir: str) -> StorageManager:
        if not save_dir or not isinstance(save_dir, str):
            raise ValueError("save_dir must be a non-empty string.")

        save_path = os.path.join(self.training_settings.base_storage_dir, save_dir)
        return StorageManager(save_path=save_path)

    def get_algorithm(self, restore_iteration: int | None) -> Algorithm:
        algorithm_configurator = AlgorithmConfigurator(
            storage_manager=self.storage_manager, training_settings=self.training_settings
        )
        return algorithm_configurator.load_algorithm(restore_iteration=restore_iteration)

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
