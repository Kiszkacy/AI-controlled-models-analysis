
using Godot;

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
}