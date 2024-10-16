import os

import numpy as np
import torch
import torch.nn.functional as func
from ray.rllib.policy.policy import Policy

from core.src.policies.agent_policy_network import AgentPolicyNetwork
from core.src.policies.critic_policy_network import CriticPolicyNetwork

LR_ACTOR = 0.001
LR_CRITIC = 0.002


class AgentPolicy(Policy):
    def __init__(self, observation_space, action_space, config):
        Policy.__init__(self, observation_space, action_space, config)
        self.actor_policy_network = AgentPolicyNetwork(observation_space.shape[0], action_space)
        self.critic_policy_network = CriticPolicyNetwork(observation_space.shape[0], action_space)
        self.target_actor = AgentPolicyNetwork(observation_space.shape[0], action_space)
        self.target_critic = CriticPolicyNetwork(observation_space.shape[0], action_space)
        self.tau = 0.005
        self.update_target(self.target_actor, self.actor_policy_network)
        self.update_target(self.target_critic, self.critic_policy_network)
        self.model_path = config["model_path"]
        #  if self.model_path is not None and os.path.exists(self.model_path):
        self.actor_policy_network.load_state_dict(torch.load(os.path.join(self.model_path, "actor.pth")))
        self.critic_policy_network.load_state_dict(torch.load(os.path.join(self.model_path, "critic.pth")))
        self.actor_optimizer: torch.optim.Optimizer = torch.optim.Adam(
            self.actor_policy_network.parameters(), lr=LR_ACTOR, maximize=True
        )
        self.critic_optimizer: torch.optim.Optimizer = torch.optim.Adam(
            self.critic_policy_network.parameters(), lr=LR_CRITIC, maximize=True
        )
        self.gamma = config["gamma"]
        self.buffer = [None]
        self.loss = []
        self.action_dim = action_space.shape[0]

    def discount_rewards(self, rewards: torch.Tensor):
        """Decreasing rewards with respect to time."""
        discounted_rewards = torch.zeros_like(rewards)
        current_reward_sum = 0
        for k, reward in enumerate(reversed(rewards)):
            current_reward_sum = current_reward_sum * self.gamma + reward
            discounted_rewards[-k - 1] = current_reward_sum

        return discounted_rewards

    def update_target(self, target_model, model):
        for target_param, param in zip(target_model.parameters(), model.parameters(), strict=False):
            target_param.data.copy_(self.tau * param.data + (1 - self.tau) * target_param.data)

    @staticmethod
    def sample_actions(mean, stddev, deterministic=False):
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
            actions = self.target_actor(obs_tensor.unsqueeze(0)) + np.random.normal(0, 0.2, size=self.action_dim)  # noqa: NPY002

            """
            actions_temp = self.sample_actions(mean, stddev)
            action_names = ["accelerate", "rotate"]

            actions = [
                {action_names[0]: action_values[0], action_names[1]: action_values[1]}
                for action_values in actions_temp[0]
            ]
            """
        return actions, [], {}

    def learn_on_loaded_batch(self, offset: int = 0, buffer_index: int = 0):  # noqa: ARG002
        data = self.buffer[buffer_index]
        rewards = data.get("rewards", None)
        states = data.get("obs", None)
        actions = data.get("actions", None)
        next_states = data.get("new_obs", None)
        dones = data.get("dones", None)

        states = torch.tensor(states, dtype=torch.float32)
        actions = torch.tensor(actions, dtype=torch.float32)
        rewards = torch.tensor(rewards, dtype=torch.float32).unsqueeze(1)
        next_states = torch.tensor(next_states, dtype=torch.float32)
        dones = torch.tensor(dones, dtype=torch.float32).unsqueeze(1)

        with torch.no_grad():
            next_actions = self.target_actor(next_states)
            target_q = self.target_critic(next_states, next_actions)
            target_values = rewards + (1 - dones) * self.gamma * target_q

        q_values = self.critic_policy_network(states, actions)
        critic_loss = func.mse_loss(q_values, target_values)

        self.critic_optimizer.zero_grad()
        critic_loss.backward()
        self.critic_optimizer.step()

        actor_loss = -self.critic_policy_network(states, self.actor_policy_network(states)).mean()

        self.actor_optimizer.zero_grad()
        actor_loss.backward()
        self.actor_optimizer.step()

        self.update_target(self.target_actor, self.actor_policy_network)
        self.update_target(self.target_critic, self.critic_policy_network)

        return {"critic_loss": critic_loss.item(), "actor_loss": actor_loss.item()}

    def get_weights(self):
        return {
            "actor": self.actor_policy_network.state_dict(),
            "critic": self.critic_policy_network.state_dict(),
            "target_actor": self.target_actor.state_dict(),
            "target_critic": self.target_critic.state_dict(),
        }

    def set_weights(self, weights):
        self.actor_policy_network.load_state_dict(weights["actor"])
        self.critic_policy_network.load_state_dict(weights["critic"])
        self.target_actor.load_state_dict(weights["target_actor"])
        self.target_critic.load_state_dict(weights["target_critic"])

        if self.model_path is not None:
            if not os.path.exists(self.model_path):
                os.makedirs(self.model_path)
            torch.save(self.actor_policy_network.state_dict(), os.path.join(self.model_path, "actor.pth"))
            torch.save(self.critic_policy_network.state_dict(), os.path.join(self.model_path, "critic.pth"))

    def load_batch_into_buffer(self, batch, buffer_index: int = 0) -> int:
        total_samples = len(batch)

        while len(self.buffer) < buffer_index:
            self.buffer.append(None)
        self.buffer[buffer_index] = batch

        return total_samples
