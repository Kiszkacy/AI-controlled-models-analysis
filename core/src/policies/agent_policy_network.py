from gymnasium.spaces import Box
from torch import nn

from core.src.policies.policy_network import PolicyNetwork


class AgentPolicyNetwork(PolicyNetwork):
    def __init__(self, input_shape: int, action_space: Box):
        super().__init__()

        output_shape = action_space.shape[0]

        self.layers = nn.Sequential(
            nn.LayerNorm(input_shape),
            nn.Linear(in_features=input_shape, out_features=200),
            nn.ReLU(),
            nn.Linear(in_features=200, out_features=128),
            nn.ReLU(),
            nn.Linear(128, output_shape),
            nn.Tanh(),
        )

    def forward(self, obs):
        obs = obs.view(obs.size(0), 3)
        return 2 * self.layers(obs)
