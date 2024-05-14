from torch import nn

from core.src.policies.policy_network import PolicyNetwork


class PongPolicyNetwork(PolicyNetwork):
    def __init__(self, input_shape: int, output_shape: int):
        super().__init__()
        self.layers = nn.Sequential(
            nn.LayerNorm(input_shape),
            nn.Linear(in_features=input_shape, out_features=200),
            nn.ReLU(),
            nn.Linear(in_features=200, out_features=output_shape),
        )

    def forward(self, x):
        return self.layers.forward(x)
