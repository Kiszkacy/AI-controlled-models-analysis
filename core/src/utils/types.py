from typing import Annotated, TypeVar

from annotated_types import Gt, IsNotFinite
from pydantic import BaseModel

PositiveInteger = Annotated[int, Gt(0)]
FiniteFloat = Annotated[float, IsNotFinite]
PositiveFiniteFloat = Annotated[float, IsNotFinite, Gt(0)]

ImmutableCls = TypeVar("ImmutableCls")


def model_to_dataclass(model: BaseModel, immutable_cls: type[ImmutableCls]) -> ImmutableCls:
    return immutable_cls(**model.dict())
