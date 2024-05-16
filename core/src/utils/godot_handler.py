import json
import subprocess
import threading
from pathlib import Path

from loguru import logger

from core.src.pipe_handler.pipe_handler import PipeHandler
from core.src.settings import get_settings


class GodotHandler:
    pipe_name_format: str = "python-godot-{thread_id}-{instance_id}"
    godot_executable: Path = get_settings().godot.godot_executable
    instance_counter: int = 0
    counter_lock: threading.Lock = threading.Lock()

    def __init__(self, project_path: str | None = None):
        self.project_path: str | Path = project_path if project_path else get_settings().godot.project_path
        self.godot_thread: threading.Thread | None = None
        self.pipe_name: str = self.get_pipe_name()
        self.pipe_handler: PipeHandler = PipeHandler(pipe_name=self.pipe_name)

    @classmethod
    def get_pipe_name(cls) -> str:
        thread_id = threading.current_thread().ident

        with cls.counter_lock:
            cls.instance_counter += 1
            return cls.pipe_name_format.format(thread_id=thread_id, instance_id=cls.instance_counter)

    def send(self, data: bytes) -> None:
        self.pipe_handler.send(data)

    def request_data(self) -> dict:
        data: bytes = self.pipe_handler.receive()
        decoded_data = data.decode()
        return json.loads(decoded_data)

    @logger.catch(reraise=True)
    def launch_godot(self):
        godot_args = [
            "--path",
            self.project_path,
            "--headless",
        ]

        project_args = [
            "--pipe-name",
            self.pipe_name,
        ]

        separator = ["++"]

        command = [self.godot_executable] + godot_args + separator + project_args

        try:
            # Run the command
            self.godot_thread = threading.Thread(target=subprocess.run, args=(command,))
            self.godot_thread.start()
        except subprocess.CalledProcessError as e:
            logger.error("Error running Godot project:", e)
        else:
            self.pipe_handler.connect()

    def __del__(self):
        self.pipe_handler.disconnect()
