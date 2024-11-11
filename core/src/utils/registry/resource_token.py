from enum import Enum, auto

import attr
from attr import field


class ResourceTokenType(Enum):
    BLOCKING = auto()
    NON_BLOCKING = auto()


@attr.define
class ResourceToken:
    type: ResourceTokenType
    active: bool = field(default=True)

    def deactivate(self):
        self.active = False
