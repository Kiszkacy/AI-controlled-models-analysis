
using Godot;

public class EnvironmentManager : Singleton<EnvironmentManager>, Initializable
{
    public Environment Environment { get; private set; }

    private readonly InitializableWrapper initialized = new();
    public bool IsInitialized => this.initialized.IsInitialized;

    public void Initialize(Environment environment)
    {
        this.Environment = environment;
        this.initialized.Initialize();
    }

    public bool IsTerrainAt(Vector2 where)
    {
        return BiomeType.Ocean != EnvironmentGenerationUtil.GetBiomeAt(where, this.Environment.Size, this.Environment.TemplateData.GenerationSettings.BiomeChunkSize, this.Environment.TemplateData.BiomeData);
    }

    public void Reset()
    {
        this.Environment = null;
        this.initialized.Reset();
    }
}