from gymnasium.spaces import Dict
from torch import exp, nn, tanh

from core.src.policies.policy_network import PolicyNetwork


class AgentPolicyNetwork(PolicyNetwork):
    def __init__(self, input_shape: int, action_space: Dict):
        super().__init__()

        output_shape = len(action_space.spaces)

        self.layers = nn.Sequential(
            nn.LayerNorm(input_shape),
            nn.Linear(in_features=input_shape, out_features=200),
            nn.ReLU(),
            nn.Linear(in_features=200, out_features=128),
            nn.ReLU(),
        )

        self.mean_layer = nn.Linear(128, output_shape)
        self.log_std_layer = nn.Linear(128, output_shape)

    def forward(self, obs):
        x = self.layers(obs)

        mean = tanh(self.mean_layer(x))
        log_std = self.log_std_layer(x)
        stddev = abs(tanh(exp(log_std)) // 2)

        return mean, stddev
