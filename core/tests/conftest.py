import pytest
import ray


@pytest.fixture(scope="session")
def ray_cluster():
    ray.shutdown()
    cluster = ray.init(num_cpus=2)
    yield cluster
    ray.shutdown()
