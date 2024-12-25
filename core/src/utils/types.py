from dataclasses import fields, is_dataclass
from typing import Annotated, TypeVar, get_args, get_origin

from annotated_types import Gt, IsNotFinite

PositiveInteger = Annotated[int, Gt(0)]
PositiveFloat = Annotated[float, Gt(0)]
FiniteFloat = Annotated[float, IsNotFinite]
PositiveFiniteFloat = Annotated[float, IsNotFinite, Gt(0)]

ImmutableCls = TypeVar("ImmutableCls")


def dict_to_dataclass(data_dict: dict, immutable_cls: type[ImmutableCls]):
    field_values = {}

    if not isinstance(data_dict, dict):
        return data_dict
    for field in fields(immutable_cls):  # type: ignore [arg-type]
        if field.name not in data_dict:
            continue

        value = data_dict[field.name]
        field_type = field.type

        if is_dataclass(field_type):
            field_values[field.name] = dict_to_dataclass(value, field_type)
        elif get_origin(field_type) is list:
            element_type = get_args(field_type)[0]
            if is_dataclass(element_type):
                field_values[field.name] = [dict_to_dataclass(item, element_type) for item in value]
            else:
                field_values[field.name] = value
        else:
            field_values[field.name] = value

    return immutable_cls(**field_values)
