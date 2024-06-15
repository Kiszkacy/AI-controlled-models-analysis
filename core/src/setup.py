import inspect
import logging
import sys
from types import FrameType

from loguru import logger


def configure_logging() -> None:
    class InterceptHandler(logging.Handler):
        def emit(self, record: logging.LogRecord) -> None:
            # Get corresponding Loguru level if it exists.
            level: str | int
            try:
                level = logger.level(record.levelname).name
            except ValueError:
                level = record.levelno

            # Find caller from where originated the logged message.
            frame: FrameType | None = inspect.currentframe()
            depth: int = 0
            while frame and (depth == 0 or frame.f_code.co_filename == logging.__file__):
                frame = frame.f_back
                depth += 1

            logger.opt(depth=depth, exception=record.exc_info).log(level, record.getMessage())

    logger.remove()
    logger.add(sys.stderr, level="INFO", backtrace=True, diagnose=True)

    logging.basicConfig(handlers=[InterceptHandler()], level=0, force=True)
    logging.getLogger("ray").handlers = [InterceptHandler()]
    logging.getLogger("ray.tune").handlers = [InterceptHandler()]
    logging.getLogger("ray.rllib").handlers = [InterceptHandler()]
    logging.getLogger("ray.train").handlers = [InterceptHandler()]
    logging.getLogger("ray.serve").handlers = [InterceptHandler()]
