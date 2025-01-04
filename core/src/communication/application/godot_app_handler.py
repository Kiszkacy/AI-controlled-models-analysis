from contextlib import AbstractContextManager
from typing import TYPE_CHECKING, Any

from core.src.communication.godot_handler import GodotHandler

if TYPE_CHECKING:
    from core.src.communication.pipe_handler import PipeHandler


class GodotAppHandler(GodotHandler[str]):
    def __init__(self, pipe_handler: "PipeHandler", *additional_resources: AbstractContextManager):
        super().__init__(pipe_handler, *additional_resources)

    def handshake(self) -> bool:
        raise NotImplementedError

    def _attempt_decode(self, data: bytes) -> Any:
        return data.decode()
