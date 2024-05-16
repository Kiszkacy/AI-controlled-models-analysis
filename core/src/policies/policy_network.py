import abc

import torch
from torch import nn


class PolicyNetwork(abc.ABC, nn.Module):
    @abc.abstractmethod
    def forward(self, x) -> torch.Tensor: ...
