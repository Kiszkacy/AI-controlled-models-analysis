
using Godot;
using Godot.Collections;

public readonly struct EnvironmentGenerationSettings
{
    public Vector2 Size { get; }
    public Vector2 BiomeChunkSize { get; }
    public Vector2 TerrainChunkSize { get; }

    public EnvironmentGenerationSettings(Vector2 size, Vector2 biomeChunkSize, Vector2 terrainChunkSize)
    {
        this.Size = size;
        this.BiomeChunkSize = biomeChunkSize;
        this.TerrainChunkSize = terrainChunkSize;
    }
    
    public Dictionary ToDictionary()
    {
        var dict = new Dictionary
        {
            { "Size", this.Size },
            { "BiomeChunkSize", this.BiomeChunkSize },
            { "TerrainChunkSize", this.TerrainChunkSize }
        };

        return dict;
    }
}