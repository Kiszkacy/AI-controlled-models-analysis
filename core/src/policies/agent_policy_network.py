from gymnasium.spaces import Box
from torch import clamp, exp, nn, tanh

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
        )

        self.mean_layer = nn.Linear(128, output_shape)
        self.log_std_layer = nn.Linear(128, output_shape)

    def forward(self, obs):
        obs = obs.view(obs.size(0), 3)
        x = self.layers(obs)

        mean = 2 * tanh(self.mean_layer(x))
        log_std = self.log_std_layer(x)
        stddev = abs(tanh(exp(log_std)) / 2)
        stddev = clamp(stddev, min=1e-6)

        return mean, stddev
