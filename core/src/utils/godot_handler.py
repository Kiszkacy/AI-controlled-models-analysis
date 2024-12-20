import json
import random
import subprocess
import threading
from pathlib import Path

from loguru import logger

from core.src.pipe_handler.pipe_handler import PipeHandler
from core.src.settings import get_settings
from core.src.workers.worker_manager import WorkerManager


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
        self.manager = WorkerManager()
        self.manager.register_worker()
        self.manager.decide_leader()

    @classmethod
    def get_pipe_name(cls) -> str:
        thread_id = threading.current_thread().ident

        with cls.counter_lock:
            cls.instance_counter += 1
            return cls.pipe_name_format.format(thread_id=thread_id, instance_id=cls.instance_counter)

    def send(self, data: bytes) -> None:
        self.pipe_handler.send(data)

    def request_data(self) -> list[dict] | int:
        data: bytes = self.pipe_handler.receive()
        try:
            decoded_data = data.decode()
            return json.loads(decoded_data)
        except json.JSONDecodeError:
            return int.from_bytes(data, byteorder="little")

    @logger.catch(reraise=True)
    def launch_godot(self) -> None:
        godot_args = [
            "--path",
            self.project_path,
        ]

        if not self.manager.is_leader():
            godot_args.append("--headless")

        random_number = random.randint(0, 10000)

        project_args = [
            "--pipe-name",
            self.pipe_name,
            "--environment-seed",
            str(random_number),
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
