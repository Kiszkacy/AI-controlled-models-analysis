
using Godot;

public readonly struct EnvironmentTemplate
{
    public EnvironmentGenerationSettings GenerationSettings { get; }
    public BiomeType[] BiomeData { get; }
    public bool[] TerrainData { get; }
    public EnvironmentObjectData[] ObjectData { get; }

    private void _Instantiate(Environment environment, bool shouldInitialize)
    {
        if (shouldInitialize)
        {
            environment.Initialize(this, true);
        }
        Node objectNode = environment.GetNode("Objects");

        foreach (EnvironmentObjectData objectData in this.ObjectData)
        {
            PackedScene packedObject = EnvironmentObjectTable.Get(objectData.Id);
            EnvironmentObject instantiatedObject = packedObject.Instantiate<EnvironmentObject>();
            instantiatedObject.GlobalPosition = objectData.Position;
            // TODO object registration, check safe spawn radius here ?
            objectNode.AddChild(instantiatedObject);
        }
    }

    public Environment Instantiate()
    {
        PackedScene packedEnvironment = ResourceLoader.Load<PackedScene>("res://src/scenes/environment/environment.tscn");
        Environment environment = packedEnvironment.Instantiate<Environment>();

        this._Instantiate(environment, true);
        return environment;
    }

    public void InstantiateInto(Environment environment)
    {
        this._Instantiate(environment, false);
    }

    public EnvironmentTemplate(EnvironmentGenerationSettings generationSettings, BiomeType[] biomeData, bool[] terrainData, EnvironmentObjectData[] objectData)
    {
        this.GenerationSettings = generationSettings;
        this.BiomeData = biomeData;
        this.TerrainData = terrainData;
        this.ObjectData = objectData;
    }
}