# Core Module

This document outlines the installation, configuration, and usage of the Core Module.

## Prerequisites:

* Python 3.11 (https://www.python.org/downloads/)
* Required Python packages listed in `requirements.txt`

## Installation

1. **Verify Python Version:** Ensure you have Python 3.11 installed. You can check the version by running `python3 --version` in your terminal.
2. **Clone Repository:** Clone this repository using Git.
3. **Install Dependencies:** Navigate to the `core` directory of the cloned repository. Install the required Python packages using pip:

```bash
pip install -Ur requirements.txt
```

4. **Install Pre-commit Hooks:**

```bash
pre-commit install
```

**Optional: GPU Acceleration**

This project uses PyTorch. If you want to leverage GPU acceleration for faster processing, refer to the PyTorch installation instructions for your specific hardware setup: https://pytorch.org/get-started/locally/

## Configuration

1. **Create Environment File:** Copy the provided `env.template` file and rename it to `.env` in the `Core` directory.
2. **Set Configuration Values:** Edit the `.env` file and provide values for the configuration settings as prompted by the comments within the file. 
Additional configuration options can be found in `settings.yaml` file.
## Usage

To run the Core Module, execute the `main.py` script in your terminal:

```bash
python main.py
```

