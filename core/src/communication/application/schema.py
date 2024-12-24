from typing import Self

from pydantic import BaseModel, DirectoryPath, FilePath, field_validator, model_validator

from core.src.utils.types import FiniteFloat, PositiveFiniteFloat, PositiveInteger


class GodotSettings(BaseModel):
    class Config:
        allow_mutation = False

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


class TrainingSettings(BaseModel):
    class Config:
        allow_mutation = False

    class Resources(BaseModel):
        class Config:
            allow_mutation = False

        number_of_workers: PositiveInteger
        number_of_env_per_worker: PositiveInteger
        training_checkpoint_frequency: PositiveInteger

    class TrainingVariables(BaseModel):
        class Config:
            allow_mutation = False

        training_iterations: PositiveInteger
        training_batch_size: PositiveInteger
        learning_rate: PositiveFiniteFloat
        entropy_coefficient: PositiveFiniteFloat


class EnvironmentSettings(BaseModel):
    class Config:
        allow_mutation = False

    observation_space_size: PositiveInteger
    observation_space_low: FiniteFloat
    observation_space_high: FiniteFloat
    action_space_range: PositiveInteger
    action_space_low: FiniteFloat
    action_space_high: FiniteFloat
    number_of_agents: PositiveInteger

    @model_validator(mode="after")
    def validate_space(self: Self) -> Self:
        if self.observation_space_low >= self.observation_space_high:
            raise ValueError(
                f"Field observation_space_low={self.observation_space_low} has to be strictly less "
                f"than field observation_space_high={self.observation_space_high}."
            )
        return self
