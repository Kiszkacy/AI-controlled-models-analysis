import json
from contextlib import AbstractContextManager
from typing import TYPE_CHECKING

from core.src.communication.godot_handler import GodotHandler

if TYPE_CHECKING:
    from core.src.communication.pipe_handler import PipeHandler


class GodotAppHandler(GodotHandler):
    def __init__(self, pipe_handler: "PipeHandler", *additional_resources: AbstractContextManager):
        super().__init__(pipe_handler, *additional_resources)

    def handshake(self) -> bool:
        raise NotImplementedError

    def _attempt_decode(self, data: bytes) -> list[dict] | None:
        try:
            decoded_data = data.decode()
            return json.loads(decoded_data)
        except json.JSONDecodeError:
            return None

    def _handle_communication_code(self, data: bytes) -> int:
        return int.from_bytes(data, byteorder="little")  # TODO: later will be changed to enum
