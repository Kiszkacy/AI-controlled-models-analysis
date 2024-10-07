from loguru import logger

from core.src.utils.shared_registry import SharedRegistry
from core.src.workers.value_objects import WorkerData, WorkerId


class WorkerManager:
    def __init__(self):
        self.worker_registry = SharedRegistry[WorkerId, WorkerData]("WorkerRegistry")
        self.worker_id: WorkerId = -1

    def is_leader(self) -> bool:
        if self.worker_id == -1:
            return False
        return self.worker_registry[self.worker_id].is_leader

    def register_worker(self) -> WorkerId:
        if self.worker_id != -1:
            return self.worker_id

        with self.worker_registry:
            worker_ids = self.worker_registry.keys()
            possible_worker_ids = set(range(len(worker_ids) + 1))
            new_worker_id = min(possible_worker_ids.difference(worker_ids))

            self.worker_registry[new_worker_id] = WorkerData(worker_id=new_worker_id, is_leader=False)

        logger.info(f"Registered worker with id {new_worker_id}")
        self.worker_id = new_worker_id
        return new_worker_id

    def decide_leader(self) -> None:
        with self.worker_registry:
            worker_keys = self.worker_registry.keys()
            worker_data = [self.worker_registry[worker_id] for worker_id in worker_keys]

            if any(data.worker_id < self.worker_id for data in worker_data):
                # There is a worker with a smaller ID, so do nothing
                return

            if any(data.is_leader for data in worker_data):
                # A leader has already been elected, so do nothing
                return

            leader_data = self.worker_registry[self.worker_id]
            leader_data.is_leader = True
            self.worker_registry[self.worker_id] = leader_data
        logger.info(f"Worker {self.worker_id} electing self as the leader.")
