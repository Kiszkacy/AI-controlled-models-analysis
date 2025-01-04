import random
import subprocess
import threading
from pathlib import Path
from typing import Annotated, Self

from loguru import logger
from pydantic import BaseModel, DirectoryPath, Field, FilePath


class GodotLaunchConfiguration(BaseModel):
    # Path leading to godot executable
    executable_path: FilePath

    # Path leading to godot project root
    project_path: DirectoryPath

    # Run godot in headless mode
    headless: bool

    # Seed used by godot
    seed: Annotated[int | None, Field(gte=0)] = None


class GodotLauncher:
    _pipe_name_format: str = "python-godot-{thread_id}-{instance_id}"
    _instance_counter: int = 0
    _counter_lock: threading.Lock = threading.Lock()

    def __init__(self, executable_path: Path, project_path: Path, headless: bool, seed: int | None = None):
        self.executable_path = executable_path
        self.project_path = project_path
        self.headless = headless
        self.seed = seed or random.randint(0, 10000)
        self._pipe_name: str | None = None

    @classmethod
    def from_configuration(cls, configuration: GodotLaunchConfiguration) -> Self:
        return cls(
            executable_path=configuration.executable_path,
            project_path=configuration.project_path,
            headless=configuration.headless,
            seed=configuration.seed,
        )

    @property
    def pipe_name(self) -> str:
        if self._pipe_name is not None:
            return self._pipe_name

        self._pipe_name = self._build_pipe_name()
        return self._pipe_name

    def launch(self) -> None:
        command = self._build_godot_command()
        self._run_command(command)

    def _build_godot_command(self) -> list[str]:
        godot_args = ["--path", str(self.project_path)]
        if self.headless:
            godot_args.append("--headless")

        project_args = ["--pipe-name", self.pipe_name, "--environment-seed", str(self.seed)]

        return [str(self.executable_path)] + godot_args + ["++"] + project_args

    def _build_pipe_name(self) -> str:
        thread_id = threading.current_thread().ident

        with self._counter_lock:
            self._instance_counter += 1
            return self._pipe_name_format.format(thread_id=thread_id, instance_id=self._instance_counter)

    @logger.catch(reraise=True)
    def _run_command(self, command: list[str]) -> None:
        self.godot_thread = threading.Thread(target=subprocess.run, args=(command,))
        self.godot_thread.start()

    @classmethod
    def get_pipe_name(cls) -> str:
        thread_id = threading.current_thread().ident

        with cls._counter_lock:
            cls._instance_counter += 1
            return cls._pipe_name_format.format(thread_id=thread_id, instance_id=cls._instance_counter)

    def disconnect(self) -> None:
        if self.godot_thread and self.godot_thread.is_alive():
            self.godot_thread.join()

    def __enter__(self):
        self.launch()

    def __exit__(self, exc_type, exc_val, exc_tb):
        self.disconnect()
