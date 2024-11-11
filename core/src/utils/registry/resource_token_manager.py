import asyncio
from typing import Self

from core.src.utils.registry.resource_token import ResourceToken, ResourceTokenType


class ResourceTokenManager:
    def __init__(self: Self):
        self.lock = asyncio.Lock()

    def acquire_token(self: Self) -> ResourceToken:
        return ResourceToken(ResourceTokenType.NON_BLOCKING)

    async def acquire_blocking_token(self: Self) -> ResourceToken:
        """As long as this token is in use, the resource will be blocked to all other token users."""
        await self.lock.acquire()
        return ResourceToken(ResourceTokenType.BLOCKING)

    def release_blocking_token(self: Self, token: ResourceToken) -> None:
        if token.type is ResourceTokenType.BLOCKING and token.active:
            token.deactivate()
            if self.lock.locked():
                self.lock.release()

    async def process_token(self: Self, token: ResourceToken) -> bool:
        if not token.active:  # Raise some error?
            return False

        if token.type is ResourceTokenType.NON_BLOCKING:  # The all tokens are blocked by blocking token
            async with self.lock:
                pass

        if token.type is ResourceTokenType.BLOCKING:  # Blocks all other tokens, but itself passes
            pass

        return True
