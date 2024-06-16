import functools
from pathlib import Path
from typing import Annotated, Self

from loguru import logger
from pydantic import DirectoryPath, Field, FilePath, ValidationError, field_validator, model_validator
from pydantic_settings import BaseSettings, PydanticBaseSettingsSource, SettingsConfigDict, YamlConfigSettingsSource

from core.src.setup import configure_logging


class GodotSettings(BaseSettings):
    godot_executable: FilePath
    project_path: DirectoryPath

    # noinspection PyNestedDecorators
    @field_validator("godot_executable", mode="after")
    @classmethod
    def validate_godot_executable(cls, value: Path) -> Path:
        if value.suffix == ".exe":
            return value

        raise ValueError(f"Path should point to an .exe file but instead pointed to {value.suffix}")


class TrainingSettings(BaseSettings):
    model_config = SettingsConfigDict(frozen=True)

    number_of_workers: Annotated[int, Field(gt=0)]
    number_of_env_per_worker: Annotated[int, Field(gt=0)]
    training_iterations: Annotated[int, Field(gt=0)]
    training_batch_size: Annotated[int, Field(gt=0)]


class EnvironmentSettings(BaseSettings):
    model_config = SettingsConfigDict(frozen=True)

    observation_space_size: Annotated[int, Field(gt=0, default=5)]
    observation_space_low: Annotated[float, ...]
    observation_space_high: Annotated[float, ...]
    action_space_range: Annotated[int, Field(gt=0, default=2)]
    number_of_agents: Annotated[int, Field(gt=0)]

    @model_validator(mode="after")
    def check_passwords_match(self) -> Self:
        if self.observation_space_low >= self.observation_space_high:
            raise ValueError(
                f"Field observation_space_low={self.observation_space_low} has to be strictly less "
                f"than field observation_space_high={self.observation_space_high}."
            )
        return self


class CoreSettings(BaseSettings):
    model_config = SettingsConfigDict(
        yaml_file="../settings.yaml",
        env_file="../.env",
        env_prefix="CORE_",
        env_nested_delimiter="__",
        env_file_encoding="utf-8",
        frozen=True,
    )

    godot: GodotSettings = Field(description="The godot settings")

    training: TrainingSettings = Field(description="Training settings")

    environment: EnvironmentSettings = Field(description="Training environment settings")

    @classmethod
    def settings_customise_sources(  # noqa: PLR0913
        cls,
        settings_cls: type[BaseSettings],
        init_settings: PydanticBaseSettingsSource,
        env_settings: PydanticBaseSettingsSource,
        dotenv_settings: PydanticBaseSettingsSource,
        file_secret_settings: PydanticBaseSettingsSource,
    ) -> tuple[PydanticBaseSettingsSource, ...]:
        return (
            init_settings,
            env_settings,
            dotenv_settings,
            YamlConfigSettingsSource(settings_cls),
            file_secret_settings,
        )


class CommunicationSettings(BaseSettings):
    model_config = SettingsConfigDict(frozen=True)

    reset: Annotated[int, ...]


class GlobalSettings(BaseSettings):
    model_config = SettingsConfigDict(
        yaml_file="../../global/config.yaml",
        frozen=True,
    )

    communication: CommunicationSettings = Field(description="Communication settings")

    @classmethod
    def settings_customise_sources(  # noqa: PLR0913
            cls,
            settings_cls: type[BaseSettings],
            init_settings: PydanticBaseSettingsSource,
            env_settings: PydanticBaseSettingsSource,
            dotenv_settings: PydanticBaseSettingsSource,
            file_secret_settings: PydanticBaseSettingsSource,
    ) -> tuple[PydanticBaseSettingsSource, ...]:
        return (
            init_settings,
            env_settings,
            dotenv_settings,
            YamlConfigSettingsSource(settings_cls),
            file_secret_settings,
        )


@functools.lru_cache(maxsize=1)
def get_settings() -> CoreSettings:
    """Loads settings."""
    configure_logging()

    try:
        settings = CoreSettings()
        logger.success("Successfully loaded core settings.")
        return settings
    except ValidationError as e:
        logger.error(f"Error loading core settings: {e}")
        raise


@functools.lru_cache(maxsize=1)
def get_global_settings() -> GlobalSettings:
    """Loads global settings."""
    configure_logging()

    try:
        settings = GlobalSettings()
        logger.success("Successfully loaded global settings.")
        return settings
    except ValidationError as e:
        logger.error(f"Error loading global settings: {e}")
        raise


def reload_settings() -> CoreSettings:
    """Reloads settings."""
    get_settings.cache_clear()
    return get_settings()


def reload_global_settings() -> GlobalSettings:
    """Reloads settings."""
    get_global_settings.cache_clear()
    return get_global_settings()
