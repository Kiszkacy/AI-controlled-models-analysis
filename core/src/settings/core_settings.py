import functools
from typing import Any, Self

from loguru import logger
from pydantic import DirectoryPath, Field, FilePath, field_validator, model_validator
from pydantic.alias_generators import to_pascal
from pydantic_settings import BaseSettings, PydanticBaseSettingsSource, SettingsConfigDict, YamlConfigSettingsSource

from core.src.settings.app_settings import APP_REGISTRY
from core.src.utils.registry.shared_registry import SharedRegistry
from core.src.utils.types import FiniteFloat, PositiveFloat, PositiveInteger


class GodotSettingsSchema(BaseSettings):
    model_config = SettingsConfigDict(frozen=True, alias_generator=to_pascal, populate_by_name=True)

    # Path leading to godot executable
    godot_executable: FilePath

    # Path leading to godot project root
    project_path: DirectoryPath

    # noinspection PyNestedDecorators
    @field_validator("godot_executable", mode="after")
    @classmethod
    def validate_godot_executable(cls, value: FilePath) -> FilePath:
        if value.suffix == ".exe":
            return value
        raise ValueError(f"Path should point to an .exe file but instead pointed to {value.suffix}")


class ConfigSettingsSchema(BaseSettings):
    model_config = SettingsConfigDict(frozen=True, alias_generator=to_pascal, populate_by_name=True)
    number_of_workers: PositiveInteger
    number_of_env_per_worker: PositiveInteger
    use_gpu: bool
    algorithm: str
    training_batch_size: PositiveInteger
    lr: PositiveFloat
    grad_clip: PositiveFloat
    gamma: PositiveFloat
    entropy_coeff: float
    clip_param: float
    lstm_cell_size: PositiveInteger
    max_seq_len: PositiveInteger
    fcnet_hiddens: list[int]


class StorageSettingsSchema(BaseSettings):
    model_config = SettingsConfigDict(frozen=True, alias_generator=to_pascal, populate_by_name=True)
    name: str
    save_path: str
    max_checkpoints: PositiveInteger
    restore_iteration: int | None


class TrainingSettingsSchema(BaseSettings):
    model_config = SettingsConfigDict(frozen=True, alias_generator=to_pascal, populate_by_name=True)

    training_iterations: PositiveInteger
    training_checkpoint_frequency: PositiveInteger
    is_resume: bool
    config_settings: ConfigSettingsSchema = Field(description="Algorithm configuration settings")


class AgentEnvironmentSettingsSchema(BaseSettings):
    model_config = SettingsConfigDict(frozen=True, alias_generator=to_pascal, populate_by_name=True)

    observation_space_size: PositiveInteger = 5
    observation_space_low: FiniteFloat
    observation_space_high: FiniteFloat
    action_space_range: PositiveInteger = 2
    action_space_low: FiniteFloat
    action_space_high: FiniteFloat
    number_of_agents: PositiveInteger
    # seed: PositiveInteger
    # use_seed: bool

    @model_validator(mode="after")
    def validate_ranges(self: Self) -> Self:
        if self.observation_space_low >= self.observation_space_high:
            raise ValueError(
                f"Field observation_space_low={self.observation_space_low} has to be strictly less "
                f"than field observation_space_high={self.observation_space_high}."
            )
        return self


class CoreSettingsSchema(BaseSettings):
    model_config = SettingsConfigDict(
        yaml_file="../settings.yaml",
        env_file="../.env",
        env_nested_delimiter="__",
        env_file_encoding="utf-8",
        alias_generator=to_pascal,
        frozen=True,
        populate_by_name=True,
    )
    godot: GodotSettingsSchema = Field(description="The godot settings")
    training: TrainingSettingsSchema = Field(description="Training settings")
    environment: AgentEnvironmentSettingsSchema = Field(description="Training environment settings")
    storage: StorageSettingsSchema = Field(description="Storage settings")

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
def get_core_settings() -> CoreSettingsSchema:
    """Loads settings."""
    registry: SharedRegistry[type, Any] = SharedRegistry(APP_REGISTRY)
    if CoreSettingsSchema not in registry:
        raise LookupError("CoreSettings were not registered.")

    settings = registry.get(CoreSettingsSchema)
    logger.success("Successfully loaded core settings.")
    return settings


def reload_core_settings() -> CoreSettingsSchema:
    """Reloads settings."""
    get_core_settings.cache_clear()
    return get_core_settings()
