import argparse
import functools
from enum import StrEnum, auto
from typing import Annotated, Any, get_type_hints

from annotated_types import MinLen
from loguru import logger
from pydantic.fields import Field, FieldInfo
from pydantic_settings import BaseSettings, SettingsConfigDict
from pydantic_settings.sources import PydanticBaseSettingsSource, SettingsError, YamlConfigSettingsSource

from core.src.utils.registry.shared_registry import SharedRegistry

APP_REGISTRY = "APP_REGISTRY"


class CommunicationSettingsSchema(BaseSettings):
    model_config = SettingsConfigDict(frozen=True)

    reset: int
    start: int
    stop: int


class CliSettingsSource(PydanticBaseSettingsSource):
    """
    A source that reads values from arguments passed to the program.
    """

    def __init__(self, settings_cls: type[BaseSettings]):
        super().__init__(settings_cls)
        self.args = self.collect_args(settings_cls)

    def collect_args(self, settings_cls: type[BaseSettings]) -> argparse.Namespace:
        parser = argparse.ArgumentParser()
        default_annotations = get_type_hints(settings_cls)

        for field_name, field_info in settings_cls.model_fields.items():
            if field_info.exclude:
                continue

            parser.add_argument(
                f"--{field_name}",
                default=field_info.default,
                type=default_annotations[field_name],
                required=False,
                help=field_info.description,
            )
        return parser.parse_args()

    def get_field_value(self, field: FieldInfo, field_name: str) -> tuple[Any, str, bool]:  # noqa: ARG002
        field_value = getattr(self.args, field_name)
        return field_value, field_name, False

    def __call__(self) -> dict[str, Any]:
        data: dict[str, Any] = {}

        for field_name, field in self.settings_cls.model_fields.items():
            try:
                field_value, field_key, value_is_complex = self.get_field_value(field, field_name)
            except Exception as e:
                raise SettingsError(
                    f'error getting value for field "{field_name}" from source "{self.__class__.__name__}"'
                ) from e

            try:
                field_value = self.prepare_field_value(field_name, field, field_value, value_is_complex)
            except ValueError as e:
                raise SettingsError(
                    f'error parsing value for field "{field_name}" from source "{self.__class__.__name__}"'
                ) from e

            if field_value is not None:
                data[field_key] = field_value

        return data


class Environment(StrEnum):
    DEVELOPMENT = auto()
    GODOT = auto()


class WorkEnvironmentSettingsSchema(BaseSettings):
    model_config = SettingsConfigDict(frozen=True)

    env: Environment = Field(default=Environment.GODOT, description="Work environment")
    pipe_name: Annotated[str, MinLen(1)] = ""  # TODO: do something with this, default should be None

    @classmethod
    def settings_customise_sources(  # noqa: PLR0913
        cls,
        settings_cls: type[BaseSettings],
        init_settings: PydanticBaseSettingsSource,
        env_settings: PydanticBaseSettingsSource,  # noqa: ARG003
        dotenv_settings: PydanticBaseSettingsSource,  # noqa: ARG003
        file_secret_settings: PydanticBaseSettingsSource,  # noqa: ARG003
    ) -> tuple[PydanticBaseSettingsSource, ...]:
        return (
            init_settings,
            CliSettingsSource(settings_cls),
        )


class AppSettingsSchema(BaseSettings):
    model_config = SettingsConfigDict(
        yaml_file=["global/config.yaml"],
        frozen=True,
    )

    communication: CommunicationSettingsSchema = Field(description="Communication settings")
    work_environment: WorkEnvironmentSettingsSchema = Field(
        description="Work environment settings", default_factory=lambda: WorkEnvironmentSettingsSchema()
    )

    @classmethod
    def settings_customise_sources(  # noqa: PLR0913
        cls,
        settings_cls: type[BaseSettings],
        init_settings: PydanticBaseSettingsSource,
        env_settings: PydanticBaseSettingsSource,  # noqa: ARG003
        dotenv_settings: PydanticBaseSettingsSource,  # noqa: ARG003
        file_secret_settings: PydanticBaseSettingsSource,  # noqa: ARG003
    ) -> tuple[PydanticBaseSettingsSource, ...]:
        return (
            init_settings,
            YamlConfigSettingsSource(settings_cls),
        )


@functools.lru_cache(maxsize=1)
def get_app_settings() -> AppSettingsSchema:
    """Loads app settings."""
    registry: SharedRegistry[type, Any] = SharedRegistry(APP_REGISTRY)
    if AppSettingsSchema not in registry:
        raise LookupError("AppSettings were not registered.")

    settings = registry.get(AppSettingsSchema)
    logger.success("Successfully loaded app settings.")
    return settings


def reload_app_settings() -> AppSettingsSchema:
    """Reloads app settings."""
    get_app_settings.cache_clear()
    return get_app_settings()
