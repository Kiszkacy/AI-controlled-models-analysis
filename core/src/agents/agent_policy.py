import os

import torch
from ray.rllib.policy.policy import Policy


class AgentPolicy(Policy):
    def __init__(self, observation_space, action_space, config):
        Policy.__init__(self, observation_space, action_space, config)
        self.policy_network = config["model"]["custom_model"](observation_space.shape[0], action_space)
        self.model_path = config["model_path"]
        if self.model_path is not None and os.path.exists(self.model_path):
            self.policy_network.load_state_dict(torch.load(self.model_path))
        self.optimizer: torch.optim.Optimizer = torch.optim.Adam(
            self.policy_network.parameters(), lr=config["learning_rate"], maximize=True
        )
        self.gamma = config["gamma"]
        self.buffer = [None]
        self.loss = []

    def discount_rewards(self, rewards: torch.Tensor):
        """Decreasing rewards with respect to time."""
        discounted_rewards = torch.zeros_like(rewards)
        current_reward_sum = 0
        for k, reward in enumerate(reversed(rewards)):
            current_reward_sum = current_reward_sum * self.gamma + reward
            discounted_rewards[-k - 1] = current_reward_sum

        return discounted_rewards

    @staticmethod
    def sample_actions(mean, stddev, deterministic=False):
        stddev = torch.clamp(stddev, min=1e-6)
        if deterministic:
            actions = mean
        else:
            normal_distribution = torch.distributions.Normal(mean, stddev)
            actions = normal_distribution.sample()
        return torch.clamp(actions, min=-1.0, max=1.0)

    def compute_actions(  # noqa: PLR0913
        self,
        obs_batch,
        state_batches=None,  # noqa: ARG002
        prev_action_batch=None,  # noqa: ARG002
        prev_reward_batch=None,  # noqa: ARG002
        info_batch=None,  # noqa: ARG002
        episodes=None,  # noqa: ARG002
        **kwargs,  # noqa: ARG002
    ):
        with torch.no_grad():
            obs_tensor = torch.tensor(obs_batch, dtype=torch.float32)
            mean, stddev = self.policy_network(obs_tensor.unsqueeze(0))

            actions_temp = self.sample_actions(mean, stddev)
            action_names = ["accelerate", "rotate"]

            actions = [
                {action_names[0]: action_values[0], action_names[1]: action_values[1]}
                for action_values in actions_temp[0]
            ]

        return actions, [], {}

    def learn_on_loaded_batch(self, offset: int = 0, buffer_index: int = 0):  # noqa: ARG002
        data = self.buffer[buffer_index]
        rewards = data.get("rewards", None)
        states = data.get("obs", None)
        actions = data.get("actions", None)

        mean, stddev = self.policy_network(torch.tensor(states, dtype=torch.float32))

        distribution = torch.distributions.Normal(mean, stddev)
        actions_tensor = torch.tensor(actions, dtype=torch.float32)
        log_probs_tensor = distribution.log_prob(actions_tensor).sum(dim=-1)
        advantages = self.discount_rewards(torch.tensor(rewards, dtype=torch.float))
        policy_loss = torch.dot(log_probs_tensor, advantages) / len(advantages)

        self.optimizer.zero_grad()
        policy_loss.backward()
        self.optimizer.step()

        self.loss.append(policy_loss.item())
        return {"policy_loss": policy_loss.item()}

    def get_weights(self):
        return self.policy_network.state_dict()

    def set_weights(self, weights):
        self.policy_network.load_state_dict(weights)
        torch.save(self.policy_network.state_dict(), self.model_path)

    def load_batch_into_buffer(self, batch, buffer_index: int = 0) -> int:
        total_samples = len(batch)

        while len(self.buffer) < buffer_index:
            self.buffer.append(None)
        self.buffer[buffer_index] = batch

        return total_samples
