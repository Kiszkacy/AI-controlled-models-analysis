from collections.abc import Generator
from time import sleep
from typing import Any

import pytest
import ray

from core.src.utils.registry.shared_registry import SharedRegistry

pytestmark = [pytest.mark.usefixtures("ray_cluster")]


@pytest.fixture()
def registry_name() -> str:
    return "test_registry"


@pytest.fixture()
def shared_registry(registry_name: str) -> Generator[SharedRegistry[str, Any], None, None]:
    shared_registry = SharedRegistry[str, Any](registry_name)
    yield shared_registry
    shared_registry.shutdown()


def test_shared_registry_single_operations(shared_registry: SharedRegistry[str, Any]) -> None:
    key = "test_key"
    value = "test_value"

    assert shared_registry.keys() == set()

    shared_registry[key] = value
    assert shared_registry[key] == value
    assert shared_registry.keys() == {key}

    key2 = "test_key2"
    value2 = "test_value2"

    shared_registry.put(key2, value2)
    assert shared_registry.get(key2) == value2
    assert shared_registry.keys() == {key, key2}


def test_shared_registry_default_value(shared_registry: SharedRegistry[str, Any]) -> None:
    key = "test_key"
    default = "default"

    assert shared_registry.get(key, default) == default


def test_shared_registry_single_operations_remote(
    registry_name: str, shared_registry: SharedRegistry[str, Any]
) -> None:
    key = "test_key"
    value = "test_value"

    @ray.remote
    def put_operation() -> None:
        local_shared_registry = SharedRegistry[str, str](registry_name)
        local_shared_registry[key] = value

    @ray.remote
    def get_operation() -> str:
        local_shared_registry: SharedRegistry[str, str] = SharedRegistry(registry_name)
        return local_shared_registry[key]

    ray.get(put_operation.remote())
    assert shared_registry[key] == value
    remote_value = ray.get(get_operation.remote())
    assert remote_value == value


def test_shared_registry_missing_key(shared_registry: SharedRegistry[str, Any]) -> None:
    with pytest.raises(KeyError):
        shared_registry.get("missing_key")

    with pytest.raises(KeyError):
        _ = shared_registry["missing_key"]


def test_shared_registry_atomic_operations(registry_name: str, shared_registry: SharedRegistry[str, Any]) -> None:
    key = "test_key"
    shared_registry[key] = 0

    @ray.remote
    def long_atomic_operation() -> None:
        local_shared_registry = SharedRegistry[str, int](registry_name)
        with local_shared_registry:
            value = local_shared_registry[key]
            sleep(0.1)
            local_shared_registry[key] = value + 1

    operations = [long_atomic_operation.remote() for _ in range(3)]
    ray.get(operations)
    assert shared_registry[key] == len(operations)
