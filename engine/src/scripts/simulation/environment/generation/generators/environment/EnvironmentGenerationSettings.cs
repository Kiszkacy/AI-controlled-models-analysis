
using Godot;
using Godot.Collections;

public struct EnvironmentGenerationSettings
{
    public Vector2 Size { get; set; }
    public Vector2 BiomeChunkSize { get; set; }
    public BiomeTable BiomeTable { get; set; }
    public Vector2 TerrainChunkSize { get; set; }
    public Vector2[] TerrainPoints { get; set; }
    public Vector2[] OceanPoints { get; set; }
    public float OceanSizeMultiplier { get; set; }

    public EnvironmentGenerationSettings(Vector2 size, Vector2 biomeChunkSize, BiomeTable biomeTable, Vector2 terrainChunkSize,
        Vector2[] terrainPoints, Vector2[] oceanPoints, float oceanSizeMultiplier)
    {
        this.Size = size;
        this.BiomeChunkSize = biomeChunkSize;
        this.BiomeTable = biomeTable;
        this.TerrainChunkSize = terrainChunkSize;
        this.TerrainChunkSize = terrainChunkSize;
        this.TerrainPoints = terrainPoints;
        this.OceanPoints = oceanPoints;
        this.OceanSizeMultiplier = oceanSizeMultiplier;
    }
}