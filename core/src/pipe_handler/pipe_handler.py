# mypy: ignore-errors
# ruff: disable

import platform
from typing import IO

from loguru import logger

ON_WINDOWS: bool = platform.system() == "Windows"
if ON_WINDOWS:
    import win32file
    import win32pipe
else:
    import os


MAX_BUFFER_SIZE: int = 2048
READ_BUFFER_SIZE: int = 512


class PipeHandler:
    def __init__(self, pipe_name: str | None = None) -> None:
        if pipe_name is None:
            pipe_name = "godot-python-pipe"
        self.pipe: int | IO | None = None
        self.pipe_path: str = self._default_pipe_prefix.format(pipe_name=pipe_name)

    @property
    def _default_pipe_prefix(self) -> str:
        return r"\\.\pipe\{pipe_name}" if ON_WINDOWS else "/tmp/{pipe_name}"

    def connect(self) -> None:
        if ON_WINDOWS:
            self.pipe = win32pipe.CreateNamedPipe(
                self.pipe_path,
                win32pipe.PIPE_ACCESS_DUPLEX,
                win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
                1,
                MAX_BUFFER_SIZE,
                MAX_BUFFER_SIZE,
                0,
                None,
            )
            win32pipe.ConnectNamedPipe(self.pipe, None)
        else:
            os.mkfifo(self.pipe_path)
            self.pipe = open(self.pipe_path, "r+")  # noqa: SIM115
        logger.info(f"Connected to the {self.pipe_path} pipe.")

    def disconnect(self) -> None:
        if ON_WINDOWS:
            win32file.CloseHandle(self.pipe)
        else:
            self.pipe.close()
        self.pipe = None

    def send(self, data_bytes) -> None:
        if ON_WINDOWS:
            win32file.WriteFile(self.pipe, data_bytes)
        else:
            self.pipe.write(data_bytes)
            self.pipe.flush()

    def receive(self) -> bytes:
        data: bytes
        if ON_WINDOWS:
            _, data = win32file.ReadFile(self.pipe, READ_BUFFER_SIZE)
        else:
            data = self.pipe.read(READ_BUFFER_SIZE)
        return data
