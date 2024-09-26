from gymnasium.spaces import Dict
from torch import nn

from core.src.policies.policy_network import PolicyNetwork


class AgentPolicyNetwork(PolicyNetwork):
    def __init__(self, input_shape: int, action_space: Dict):
        super().__init__()

        self.output_shape = len(action_space.spaces)

        self.layers = nn.Sequential(
            nn.LayerNorm(input_shape),
            nn.Linear(in_features=input_shape, out_features=200),
            nn.ReLU(),
            nn.Linear(in_features=200, out_features=self.output_shape),
        )

    def forward(self, x):
        return self.layers.forward(x)
