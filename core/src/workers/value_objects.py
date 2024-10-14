from typing import Annotated

from annotated_types import Ge, IsFinite
from attr import define

WorkerId = Annotated[int, Ge(0), IsFinite]


@define(slots=False)
class WorkerData:
    worker_id: WorkerId
    is_leader: bool
