
using System.IO;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Config : Singleton<Config>
{
    private static readonly string localConfigPath = "./src/config.yaml";
    private static readonly string globalConfigPath = "../global/config.yaml";

    public ConfigData Data { get; }

    public PipeConfig Pipe => this.Data.Pipe;
    public EngineConfig Engine => this.Data.Engine;
    public TestsConfig Tests => this.Data.Tests;
    public EnvironmentConfig Environment => this.Data.Environment;
    public GlobalConfig Global => this.Data.Global;
    public DisplayData Display => this.Data.Display;
    public ControlsData Controls => this.Data.Controls;
    public SavePathData Save => this.Data.Save;

    private Config()
    {
        this.Data = ConfigData.Load(localConfigPath, globalConfigPath);
    }
}

public class ConfigData
{
    public PipeConfig Pipe { get; set; } = new();
    public EngineConfig Engine { get; set; } = new();
    public TestsConfig Tests { get; set; } = new();
    public EnvironmentConfig Environment { get; set; } = new();
    public GlobalConfig Global { get; set; } = new();
    public DisplayData Display { get; set; } = new();
    public ControlsData Controls { get; set; } = new();
    public SavePathData Save { get; set; } = new();

    public static ConfigData Load(string localConfigPath, string globalConfigPath)
    {
        ConfigData configData = LoadLocalConfig(localConfigPath);
        GlobalConfig globalConfig = LoadGlobalConfig(globalConfigPath);
        configData.Global = globalConfig;
        return configData;
    }

    private static ConfigData LoadLocalConfig(string path)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        string yaml = File.ReadAllText(path);
        ConfigData config = deserializer.Deserialize<ConfigData>(yaml);
        return config;
    }

    private static GlobalConfig LoadGlobalConfig(string path)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        string yaml = File.ReadAllText(path);
        GlobalConfig config = deserializer.Deserialize<GlobalConfig>(yaml);
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
    public float EnergyUsedReproduction { get; set; }
    public float HealthLossPerSecond { get; set; }
    public float HealthRegenPerSecond { get; set; }
    public ScoreConfig Score { get; set; }
    public int BucketSize { get; set; }
    public int AgentSightProcessEveryNthFrame { get; set; }
    public int AgentWaterPenaltyUpdateEveryNthFrame { get; set; }
    public int SupervisorAgentSpawnSafeDistance { get; set; }
    public int SupervisorAgentMaxSpawnTryCount { get; set; }
    public int Seed { get; set; }
}

public class ScoreConfig
{
    public float FoodEaten { get; set; }
    public float EnergyMax { get; set; }
    public float HealthMax { get; set; }
    public float Reproduction { get; set; }
    public float WaterPenalty { get; set; }
}

public class GlobalConfig
{
    public CommunicationConfig Communication { get; set; }
}

public class CommunicationConfig
{
    public int Reset { get; set; }
}