This readme file contains basic setup information. 
For further details, please refer to [this link](https://github.com/Kiszkacy/AI-controlled-models-analysis/wiki/Core-module).


## Prerequisites

* Python 3.11 (https://www.python.org/downloads/)
* Required Python packages listed in `requirements.txt`

## Setup

#### **Verify Python Version** 
Ensure you have Python 3.11 installed. You can check the version by running `python3 --version` in your terminal.

#### **Install Dependencies**
Navigate to the `core` directory of the cloned repository. Install the required Python packages using pip:
```bash
pip install -Ur requirements.txt
```

#### **Install Pre-commit Hooks _(Developers only)_**
```bash
pre-commit install
```

#### **Create Environment File** 
Copy the provided `env.template` file and rename it to `.env` inside the `core` directory.

#### **Set Configuration Values** 
Edit the `.env` file and provide values for the configuration settings as prompted by the comments within the file. 
Additional configuration options can be found in `settings.yaml` file.

***

**Optional: GPU Acceleration**

This project uses PyTorch. If you want to leverage GPU acceleration for faster processing, refer to the PyTorch installation instructions for your specific hardware setup: https://pytorch.org/get-started/locally/

