import functools
from typing import Self

from loguru import logger
from pydantic import BaseModel, Field, ValidationError

from core.src.settings.app_settings import (
    AppSettings,
    CommunicationSettings,
    WorkEnvironmentSettings,
    get_app_settings,
    reload_app_settings,
)
from core.src.settings.core_settings import (
    AgentEnvironmentSettings,
    CoreSettings,
    GodotSettings,
    TrainingSettings,
    get_core_settings,
    reload_core_settings,
)
from core.src.setup import configure_logging


class Settings(BaseModel):
    godot: GodotSettings = Field(description="The godot settings")
    training: TrainingSettings = Field(description="Training settings")
    environment: AgentEnvironmentSettings = Field(description="Training environment settings")
    communication: CommunicationSettings = Field(description="Communication settings")
    work_environment: WorkEnvironmentSettings = Field(
        description="Work environment settings"
    )  # Should the app need to be aware of this?

    @classmethod
    def from_modules(cls, core_settings: CoreSettings, app_settings: AppSettings) -> Self:
        return cls(
            godot=core_settings.godot,
            training=core_settings.training,
            environment=core_settings.environment,
            communication=app_settings.communication,
            work_environment=app_settings.work_environment,
        )


@functools.lru_cache(maxsize=1)
def get_settings() -> Settings:
    """Loads settings."""
    configure_logging()

    try:
        core_settings = get_core_settings()
        app_settings = get_app_settings()
        settings = Settings.from_modules(core_settings, app_settings)
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
