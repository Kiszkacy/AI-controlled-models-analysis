from abc import ABC, abstractmethod
from collections.abc import Callable
from contextlib import AbstractContextManager, ExitStack
from enum import Enum
from typing import Any

from core.src.communication.pipe_handler import PipeHandler
from core.src.settings import get_settings


class CommunicationCode(Enum):
    RESET = get_settings().communication.reset

    @classmethod
    def from_bytes(cls, data: bytes) -> "CommunicationCode":
        code_value = int.from_bytes(data, byteorder="little")
        return cls(code_value)


class GodotHandler(ABC):
    def __init__(self, pipe_handler: PipeHandler, *additional_resources: AbstractContextManager) -> None:
        """
        Abstract base class for handling pipe-based communication and resource management.

        :param pipe_handler: Pipe handler used for communication.
        :param additional_resources: Additional context-managed resources that need to be acquired
                                     and released during the handler's lifecycle.
        """
        self.pipe_handler = pipe_handler
        self.resources = [*additional_resources, pipe_handler]
        self._release_resources: Callable[[], Any] = lambda: None

    def send(self, data: bytes) -> None:
        self.pipe_handler.send(data)

    def receive(self) -> list[dict] | CommunicationCode:
        data: bytes = self.pipe_handler.receive()
        decoded_data = self._attempt_decode(data)
        if decoded_data is None:
            return self._handle_communication_code(data)
        return decoded_data

    @abstractmethod
    def _attempt_decode(self, data: bytes) -> list[dict] | None: ...

    @abstractmethod
    def _handle_communication_code(self, data: bytes) -> CommunicationCode: ...

    def release_resources(self) -> None:
        self._release_resources()

    def acquire_resources(self) -> None:
        with ExitStack() as stack:
            for resource in self.resources:
                stack.enter_context(resource)
            # Method responsible for exiting all context managers implemented by resources
            self._release_resources = stack.pop_all().close

    def __enter__(self):
        self.acquire_resources()

    def __exit__(self, exc_type, exc_val, exc_tb):
        self.release_resources()
