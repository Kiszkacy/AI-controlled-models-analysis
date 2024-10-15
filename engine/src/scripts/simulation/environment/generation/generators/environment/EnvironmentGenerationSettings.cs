
using Godot;
using Godot.Collections;

public struct EnvironmentGenerationSettings
{
    public Vector2 Size { get; set; }
    public Vector2 BiomeChunkSize { get; set; }
    public Vector2 TerrainChunkSize { get; set; }

    public EnvironmentGenerationSettings(Vector2 size, Vector2 biomeChunkSize, Vector2 terrainChunkSize)
    {
        this.Size = size;
        this.BiomeChunkSize = biomeChunkSize;
        this.TerrainChunkSize = terrainChunkSize;
    }
}