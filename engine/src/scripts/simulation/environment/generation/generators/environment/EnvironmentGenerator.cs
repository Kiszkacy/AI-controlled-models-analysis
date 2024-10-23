
using Godot;

public class EnvironmentGenerator
{
    public BiomeGenerator BiomeGenerator { get; }
    public TerrainGenerator TerrainGenerator { get; }
    public ObjectGenerator ObjectGenerator { get; }
    public Vector2 Size { get; }
    public Vector2 BiomeChunkSize { get; }
    public Vector2 TerrainChunkSize { get; }
    public Vector2[] TerrainPoints { get; }
    public Vector2[] OceanPoints { get; }
    public float TerrainOceanRatio { get; }

    public EnvironmentTemplate Generate()
    {
        EnvironmentGenerationSettings settings = new(this.Size, this.BiomeChunkSize, this.TerrainChunkSize, this.TerrainPoints, this.OceanPoints, this.TerrainOceanRatio);
        BiomeType[] biomeData = this.BiomeGenerator.Generate(settings);
        bool[] terrainData = this.TerrainGenerator.Generate(settings, biomeData);
        EnvironmentObjectData[] objectData = this.ObjectGenerator.Generate(settings, biomeData, terrainData);
        return new EnvironmentTemplate(settings, biomeData, terrainData, objectData);
    }

    public EnvironmentGenerator(
        BiomeGenerator biomeGenerator, TerrainGenerator terrainGenerator, ObjectGenerator objectGenerator, Vector2 size, Vector2 biomeChunkSize, Vector2 terrainChunkSize,
        Vector2[] terrainPoints, Vector2[] oceanPoints, float terrainOceanRatio)
    {
        this.BiomeGenerator = biomeGenerator;
        this.TerrainGenerator = terrainGenerator;
        this.ObjectGenerator = objectGenerator;
        this.Size = size;
        this.BiomeChunkSize = biomeChunkSize;
        this.TerrainChunkSize = terrainChunkSize;
        this.TerrainPoints = terrainPoints;
        this.OceanPoints = oceanPoints;
        this.TerrainOceanRatio = terrainOceanRatio;
    }
}