from __future__ import annotations

from collections.abc import Hashable
from typing import Any, Generic, Self, TypeVar

import ray

from core.src.utils.registry.resource_token import ResourceToken
from core.src.utils.registry.resource_token_manager import ResourceTokenManager

__all__ = ("RemoteSharedRegistry", "SharedRegistry")

KeyType = TypeVar("KeyType", bound=Hashable)
ValueType = TypeVar("ValueType")


@ray.remote
class RemoteSharedRegistry(Generic[KeyType, ValueType], ResourceTokenManager):
    def __init__(self: Self):
        super().__init__()
        self.registry: dict[KeyType, ValueType] = {}

    async def put(self: Self, key: KeyType, value: ValueType, token: ResourceToken) -> None:
        await self.process_token(token)
        self.registry[key] = value

    async def get(self: Self, key: KeyType, token: ResourceToken, *default: Any) -> ValueType | Any:
        if len(default) > 1:
            raise TypeError(f"get expected at most 3 arguments, got {len(default) + 2}")

        await self.process_token(token)
        if default:
            return self.registry.get(key, *default)
        return self.registry[key]

    async def keys(self: Self, token: ResourceToken) -> set[KeyType]:
        await self.process_token(token)
        return set(self.registry.keys())


class SharedRegistry(Generic[KeyType, ValueType]):
    namespace: str = "registry"

    def __init__(self: Self, name: str) -> None:
        self.initialize_registry(name)
        self._registry = ray.get_actor(name, namespace=self.namespace)
        self.token = ray.get(self._registry.acquire_token.remote())

    def put(self: Self, key: KeyType, value: ValueType) -> None:
        ray.get(self._registry.put.remote(key, value, self.token))

    def get(self: Self, key: KeyType, *args: ValueType) -> ValueType:
        """
        :param key: Key
        :param args: Optional default value
        :return: Value
        """
        if len(args) > 1:
            raise TypeError(f"get expected at most 2 arguments, got {len(args)}")

        return ray.get(self._registry.get.remote(key, self.token, *args))

    def keys(self: Self) -> set[KeyType]:
        return ray.get(self._registry.keys.remote(self.token))

    @classmethod
    def initialize_registry(cls: type[SharedRegistry[KeyType, ValueType]], name: str) -> None:
        """
        Create a named ray actor of `RemoteSharedRegistry`.

        Has to be called before the first use of RemoteSharedRegistry.
        """
        RemoteSharedRegistry.options(  # type: ignore[attr-defined]
            name=name, namespace=cls.namespace, lifetime="detached", get_if_exists=True
        ).remote()

    def shutdown(self: Self) -> None:
        ray.kill(self._registry)

    def __getitem__(self: Self, key: KeyType) -> ValueType:
        return self.get(key)

    def __contains__(self, item):
        return item in self.keys()

    def __setitem__(self: Self, key: KeyType, value: ValueType) -> None:
        self.put(key, value)

    def __enter__(self: Self) -> None:
        self.token = ray.get(self._registry.acquire_blocking_token.remote())

    def __exit__(self, exc_type, exc_val, exc_tb) -> None:
        ray.get(self._registry.release_blocking_token.remote(self.token))
        self.token = ray.get(self._registry.acquire_token.remote())
