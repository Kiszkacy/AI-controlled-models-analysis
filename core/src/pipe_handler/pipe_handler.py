import sys
from typing import IO

from loguru import logger

if sys.platform == "win32":
    import win32file
    import win32pipe
    import win32security
else:
    import os


MAX_BUFFER_SIZE: int = 8192
READ_BUFFER_SIZE: int = 8192


class PipeHandler:
    def __init__(self, pipe_name: str | None = None) -> None:
        if pipe_name is None:
            pipe_name = "godot-python-pipe"
        self.pipe: int | IO | None = None
        self.pipe_path: str = self._default_pipe_prefix.format(pipe_name=pipe_name)

    @property
    def _default_pipe_prefix(self) -> str:
        return r"\\.\pipe\{pipe_name}" if sys.platform == "win32" else "/tmp/{pipe_name}"

    def _is_windows_pipe_handle(self) -> bool:
        return type(self.pipe).__name__ == "PyHANDLE"

    def connect(self) -> None:
        if sys.platform == "win32":
            security_attributes = win32security.SECURITY_ATTRIBUTES()
            security_attributes.bInheritHandle = True

            self.pipe = win32pipe.CreateNamedPipe(
                self.pipe_path,
                win32pipe.PIPE_ACCESS_DUPLEX,
                win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
                1,
                MAX_BUFFER_SIZE,
                MAX_BUFFER_SIZE,
                0,
                security_attributes,
            )

            if self.pipe == win32file.INVALID_HANDLE_VALUE:
                raise RuntimeError(f"Failed to create named pipe: {self.pipe_path}")

            win32pipe.ConnectNamedPipe(self.pipe, None)
        else:
            if sys.platform != "win32":
                if not os.path.exists(self.pipe_path):
                    os.mkfifo(self.pipe_path)
            with open(self.pipe_path, "r+") as pipe:
                self.pipe = pipe
        logger.info(f"Connected to the {self.pipe_path} pipe.")

    def disconnect(self) -> None:
        if not self.pipe:
            return

        if isinstance(self.pipe, IO):
            self.pipe.close()

        elif sys.platform == "win32" and (isinstance(self.pipe, int) or self._is_windows_pipe_handle()):
            win32file.CloseHandle(self.pipe)

        self.pipe = None

    def send(self, data_bytes) -> None:
        if not self.pipe:
            return

        if isinstance(self.pipe, IO):
            self.pipe.write(data_bytes)
            self.pipe.flush()

        elif sys.platform == "win32" and (isinstance(self.pipe, int) or self._is_windows_pipe_handle()):
            win32file.WriteFile(self.pipe, data_bytes)

    def receive(self) -> bytes:
        if not self.pipe:
            return b""

        if isinstance(self.pipe, IO):
            return self.pipe.read(READ_BUFFER_SIZE)

        if sys.platform == "win32" and (isinstance(self.pipe, int) or self._is_windows_pipe_handle()):
            _, data = win32file.ReadFile(self.pipe, READ_BUFFER_SIZE)
            if isinstance(data, str):
                return data.encode("utf-8")
            if isinstance(data, bytes):
                return data
            raise RuntimeError(f"Unexpected data type: {type(data)} received from pipe")

        return b""
