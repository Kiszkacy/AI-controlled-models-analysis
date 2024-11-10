import json
from contextlib import AbstractContextManager

from core.src.communication.environment.godot_launcher import GodotLaunchConfiguration, GodotLauncher
from core.src.communication.godot_handler import GodotHandler
from core.src.communication.pipe_handler import PipeHandler
from core.src.settings.settings import GodotSettings
from core.src.workers.worker_manager import WorkerManager


class GodotEnvironmentHandler(GodotHandler[list[dict]]):
    def __init__(
        self, pipe_handler: PipeHandler, godot_launcher: GodotLauncher, *additional_resources: AbstractContextManager
    ):
        super().__init__(pipe_handler, godot_launcher, *additional_resources)

    def _attempt_decode(self, data: bytes) -> list[dict] | None:
        try:
            decoded_data = data.decode()
            return json.loads(decoded_data)
        except json.JSONDecodeError:
            return None


def create_godot_environment(godot_settings: GodotSettings) -> GodotEnvironmentHandler:
    worker_manager = WorkerManager()
    worker_manager.register_worker()
    worker_manager.decide_leader()

    launcher_configuration = GodotLaunchConfiguration(
        executable_path=godot_settings.godot_executable,
        project_path=godot_settings.project_path,
        headless=not worker_manager.is_leader(),
        seed=None,
    )
    launcher = GodotLauncher.from_configuration(launcher_configuration)
    pipe_handler = PipeHandler(launcher.pipe_name)

    return GodotEnvironmentHandler(pipe_handler, launcher)
