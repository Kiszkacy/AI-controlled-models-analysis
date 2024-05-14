## Configuration/Setup

- **Python 3.11**: Make sure you have Python 3.11 or newer installed. You can download the latest version from [here](https://www.python.org/downloads/windows/).
- **Godot 4.2 (Mono with C# support)**: Ensure you have Godot 4.2 or newer with Mono support installed, you can download it from [here](https://godotengine.org/download/windows/). Godot does not require installation; simply extract the downloaded archive, and you'll have the executable ready to use. However, additional dependencies may be needed, such as the [.NET SDK](https://dotnet.microsoft.com/en-us/download).

Feel free to use any text editor for the entire project. However, when working on engine-related tasks, it's recommended to use editors like Visual Studio Code or JetBrains Rider. They offer plugins that ensure seamless integration with Godot.

## File Structure

```
./
│
├── engine/                        # directory related to the engine
│   ├── src/                       # source code for the engine
│   │   ├── ...
│   ├── tests/                     # tests for the engine
│   │   ├── ...
│   └── README.md                  # documentation for the engine
│
├── core/                          # directory related to the Python process
│   ├── src/                       # source code for the Python process
│   │   ├── ...
│   ├── tests/                     # tests for the Python process
│   │   ├── ...
│   └── README.md                  # documentation for the Python process
│
└── README.md                      # main project documentation
```

It's important to follow consistent naming conventions within the project structure. In the 'core' directory, files and directories should be named using the snake_case convention. In the 'engine' section, files containing code should adhere to the PascalCase convention, while other files and directories should follow the camelCase convention.