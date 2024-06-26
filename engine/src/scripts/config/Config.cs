
using System.IO;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Config : Singleton<Config>
{
    private readonly string configPath = "./src/config.yaml";

    public ConfigData Data { get; }

    public PipeConfig Pipe => this.Data.Pipe;
    public EngineConfig Engine => this.Data.Engine;
    public TestsConfig Tests => this.Data.Tests;
    public EnvironmentConfig Environment => this.Data.Environment;

    private Config()
    {
        this.Data = ConfigData.Load(this.configPath);
    }
}

public class ConfigData
{
    public PipeConfig Pipe { get; set; } = new();
    public EngineConfig Engine { get; set; } = new();
    public TestsConfig Tests { get; set; } = new();
    public EnvironmentConfig Environment { get; set; } = new();

    public static ConfigData Load(string path)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        string yaml = File.ReadAllText(path);
        ConfigData config = deserializer.Deserialize<ConfigData>(yaml);
        return config;
    }
}

public class PipeConfig
{
    public string Name { get; set; }
    public int BufferSize { get; set; }
}

public class EngineConfig
{
    public float TimeScale { get; set; }
    public int TicksPerSecond { get; set; }
}

public class TestsConfig
{
    public bool RunTests { get; set; }
    public bool RunTestsWhenOpenedViaCommandLine { get; set; }
    public bool RunSlowTests { get; set; }
    public bool PassUncertainTestsWhenFailed { get; set; }
    public bool PrintAdditionalLogs { get; set; }
}

public class EnvironmentConfig
{
    public float EnergyBaseLossPerSecond { get; set; }
    public float EnergyLossPerSecondPer100UnitsOfMovement { get; set; }
    public float EnergyLossPerSecondTurn { get; set; }
    public float HealthLossPerSecond { get; set; }
    public float HealthRegenPerSecond { get; set; }

    public ScoreConfig Score { get; set; }
}

public class ScoreConfig
{
    public float FoodEaten { get; set; }
}