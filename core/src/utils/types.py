from typing import Annotated

from annotated_types import Gt, IsNotFinite

PositiveInteger = Annotated[int, Gt(0)]
FiniteFloat = Annotated[float, IsNotFinite]
PositiveFiniteFloat = Annotated[float, IsNotFinite, Gt(0)]
