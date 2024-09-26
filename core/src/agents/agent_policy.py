import os

import numpy as np
import torch
from gymnasium.spaces import Box
from ray.rllib.policy.policy import Policy


class AgentPolicy(Policy):
    def __init__(self, observation_space, action_space, config):
        Policy.__init__(self, observation_space, action_space, config)
        self.policy_network = config["model"]["custom_model"](observation_space.shape[0], action_space)
        self.model_path = config["model_path"]
        if self.model_path is not None and os.path.exists(self.model_path):
            self.policy_network.load_state_dict(torch.load(self.model_path))
        self.optimizer = torch.optim.Adam(self.policy_network.parameters(), lr=config["learning_rate"], maximize=True)
        self.gamma = config["gamma"]
        self.buffer = [None]
        self.loss = []

    def discount_rewards(self, rewards: torch.Tensor):
        """Decreasing rewards with respect to time."""
        discounted_rewards = torch.zeros_like(rewards)
        current_reward_sum = 0
        for k, reward in enumerate(reversed(rewards)):
            current_reward_sum = current_reward_sum * self.gamma + reward
            discounted_rewards[-k - 1] = current_reward_sum  # we start at the last reward

        return discounted_rewards

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
        actions = []
        with torch.no_grad():
            for obs in obs_batch:
                obs_tensor = torch.tensor(obs, dtype=torch.float32)
                policy_logits = self.policy_network(obs_tensor.unsqueeze(0))

                action_dict = {}
                start_idx = 0
                for key, space in self.action_space.spaces.items():
                    if isinstance(space, Box):
                        if len(space.shape) > 0:
                            logits_for_key = policy_logits[:, start_idx : start_idx + np.prod(space.shape)]
                            start_idx = start_idx + np.prod(space.shape)
                        else:
                            logits_for_key = policy_logits[:, start_idx : start_idx + 1]
                            start_idx += 1

                        mean = logits_for_key.squeeze().numpy()

                        distribution = torch.distributions.Normal(loc=torch.tensor(mean), scale=torch.tensor(1.0))
                        sampled_action = distribution.sample().numpy()

                        action = np.clip(sampled_action, space.low, space.high)

                        if len(space.shape) == 0:
                            action = action.item()
                    else:
                        raise NotImplementedError(f"Action space type {type(space)} not implemented.")

                    action_dict[key] = action

                actions.append(action_dict)
        return actions, [], {}

    def learn_on_loaded_batch(self, offset: int = 0, buffer_index: int = 0):  # noqa: ARG002
        data = self.buffer[buffer_index]
        rewards = data.get("rewards", None)
        states = data.get("obs", None)
        actions = data.get("actions", None)

        policy_logits = self.policy_network(torch.tensor(states, dtype=torch.float32))

        mean, log_std = torch.chunk(policy_logits, 2, dim=-1)
        std = torch.exp(log_std)
        distribution = torch.distributions.Normal(mean, std)

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
        os.makedirs("model", exist_ok=True)
        self.policy_network.load_state_dict(weights)
        torch.save(self.policy_network.state_dict(), self.model_path)

    def load_batch_into_buffer(self, batch, buffer_index: int = 0) -> int:
        num_devices = self.config["num_workers"]
        total_samples = buffer_index
        samples_per_device = total_samples // num_devices

        while len(self.buffer) < buffer_index:
            self.buffer.append(None)
        self.buffer[buffer_index] = batch

        return samples_per_device
