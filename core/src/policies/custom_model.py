import torch
from ray.rllib.models.torch.torch_modelv2 import TorchModelV2
from torch import nn


class CustomModel(TorchModelV2, nn.Module):
    def __init__(self, obs_space, action_space, num_outputs, model_config, name):  # noqa: PLR0913
        TorchModelV2.__init__(self, obs_space, action_space, num_outputs, model_config, name)
        nn.Module.__init__(self)

        self.fc1 = nn.Linear(obs_space.shape[0], 64)
        self.fc2 = nn.Linear(64, 64)
        self.dropout = nn.Dropout(p=0.5)
        self.out = nn.Linear(64, num_outputs)

        self.value_layer = nn.Linear(64, 1)

        self._features = None

    def forward(self, input_dict, state, seq_lens):  # noqa: ARG002
        x = torch.relu(self.fc1(input_dict["obs"]))
        x = self.dropout(torch.relu(self.fc2(x)))
        self._features = x
        return self.out(x), state

    def value_function(self):
        return self.value_layer(self._features).squeeze(-1)
