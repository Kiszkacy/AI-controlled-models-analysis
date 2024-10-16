from gymnasium.spaces import Box
from torch import cat, nn, relu


class CriticPolicyNetwork(nn.Module):
    def __init__(self, input_shape: int, action_space: Box):
        super().__init__()

        output_shape = action_space.shape[0]

        self.fc1 = nn.Linear(input_shape, 128)
        self.fc2 = nn.Linear(128 + output_shape, 128)
        self.fc3 = nn.Linear(128, 1)

    def forward(self, state, action):
        state = state.view(state.size(0), 3)
        x = relu(self.fc1(state))
        x = cat([x, action], dim=1)
        x = relu(self.fc2(x))
        return self.fc3(x)
