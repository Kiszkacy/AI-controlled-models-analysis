import functools
from dataclasses import dataclass
from pathlib import Path
from typing import Self

from loguru import logger
from pydantic import ValidationError

from core.src.settings.app_settings import (
    AppSettingsSchema,
    Environment,
    get_app_settings,
    reload_app_settings,
)
from core.src.settings.core_settings import (
    CoreSettingsSchema,
    get_core_settings,
    reload_core_settings,
)
from core.src.utils.types import model_to_dataclass


@dataclass(frozen=True)
class GodotSettings:
    godot_executable: Path
    project_path: Path


@dataclass(frozen=True)
class TrainingSettings:
    number_of_workers: int
    number_of_environments_per_worker: int
    training_iterations: int
    training_batch_size: int
    training_checkpoint_frequency: int


@dataclass(frozen=True)
class AgentEnvironmentSettings:
    observation_space_size: int
    observation_space_low: float
    observation_space_high: float
    action_space_range: int
    action_space_low: float
    action_space_high: float
    number_of_agents: int


@dataclass(frozen=True)
class WorkEnvironmentSettings:
    env: Environment
    pipe_name: str | None


@dataclass(frozen=True)
class CommunicationCodes:
    reset: int
    start: int
    stop: int


@dataclass(frozen=True)
class Settings:
    godot: GodotSettings
    training: TrainingSettings
    environment: AgentEnvironmentSettings
    communication_codes: CommunicationCodes
    work_environment: WorkEnvironmentSettings

    @classmethod
    def from_schema(cls, core_settings: CoreSettingsSchema, app_settings: AppSettingsSchema) -> Self:
        return cls(
            godot=model_to_dataclass(core_settings.godot, GodotSettings),
            training=model_to_dataclass(core_settings.training, TrainingSettings),
            environment=model_to_dataclass(core_settings.environment, AgentEnvironmentSettings),
            communication_codes=model_to_dataclass(app_settings.communication, CommunicationCodes),
            work_environment=model_to_dataclass(app_settings.work_environment, WorkEnvironmentSettings),
        )


@functools.lru_cache(maxsize=1)
def get_settings() -> Settings:
    """Loads settings."""
    try:
        core_settings = get_core_settings()
        app_settings = get_app_settings()
        settings = Settings.from_schema(core_settings, app_settings)
        logger.success("Successfully loaded all settings.")
        return settings
    except ValidationError as e:
        logger.error(f"Error loading settings: {e}")
        raise


def reload_settings() -> Settings:
    """Reloads settings."""
    reload_core_settings()
    reload_app_settings()
    get_settings.cache_clear()
    return get_settings()
