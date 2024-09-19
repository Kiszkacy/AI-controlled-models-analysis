
using Godot;

public class EnvironmentGenerator
{
    private readonly BiomeGenerator biomeGenerator;
    private readonly TerrainGenerator terrainGenerator;
    private readonly ObjectGenerator objectGenerator;
    private readonly Vector2 size;
    private readonly Vector2 biomeChunkSize;
    private readonly Vector2 terrainChunkSize;
    
    public BiomeGenerator BiomeGenerator => this.biomeGenerator;
    public TerrainGenerator TerrainGenerator => this.terrainGenerator;
    public ObjectGenerator ObjectGenerator => this.objectGenerator;
    public Vector2 Size => this.size;
    public Vector2 BiomeChunkSize => this.biomeChunkSize;
    public Vector2 TerrainChunkSize => this.terrainChunkSize;

    public EnvironmentTemplate Generate()
    {
        EnvironmentGenerationSettings settings = new(this.size, this.biomeChunkSize, this.terrainChunkSize);
        BiomeType[] biomeData = this.biomeGenerator.Generate(settings);
        bool[] terrainData = this.terrainGenerator.Generate(settings, biomeData);
        EnvironmentObjectData[] objectData = this.objectGenerator.Generate(settings, biomeData, terrainData);
        return new EnvironmentTemplate(settings, biomeData, terrainData, objectData);
    }

    public EnvironmentGenerator(
        BiomeGenerator biomeGenerator, TerrainGenerator terrainGenerator, ObjectGenerator objectGenerator, Vector2 size, Vector2 biomeChunkSize, Vector2 terrainChunkSize)
    {
        this.biomeGenerator = biomeGenerator;
        this.terrainGenerator = terrainGenerator;
        this.objectGenerator = objectGenerator;
        this.size = size;
        this.biomeChunkSize = biomeChunkSize;
        this.terrainChunkSize = terrainChunkSize;
    }
}
