using System.IO;

using Godot;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class PythonConfigurationData
{
    public PythonGodotData Godot { get; set; } = new();
    public PythonTrainingData Training { get; set; } = new();
    public PythonEnvironmentData Environment { get; set; } = new();

    public static PythonConfigurationData Load(string path)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        string yaml = File.ReadAllText(path);
        PythonConfigurationData configurationData = deserializer.Deserialize<PythonConfigurationData>(yaml);
        return configurationData;
    }
}

public class PythonGodotData
{
    public string GodotExecutable { get; set; }
    public string ProjectPath { get; set; }
}

public class PythonTrainingData
{
    public int NumberOfWorkers { get; set; }
    public int NumberOfEnvironmentsPerWorker { get; set; }
    public int TrainingIterations { get; set; }
    public int TrainingBatchSize { get; set; }
    public int TrainingCheckpointFrequency { get; set; }
}

public class PythonEnvironmentData
{
    public int ObservationSpaceSize { get; set; }
    public float ObservationSpaceLow { get; set; }
    public float ObservationSpaceHigh { get; set; }
    public float ActionSpaceRange { get; set; }
    public float ActionSpaceLow { get; set; }
    public float ActionSpaceHigh { get; set; }
    public float NumberOfAgents { get; set; }
}
