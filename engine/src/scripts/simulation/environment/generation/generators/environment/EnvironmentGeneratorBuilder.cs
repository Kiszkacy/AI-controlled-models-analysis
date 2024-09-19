
using Godot;

public class EnvironmentGeneratorBuilder
{
    private BiomeGenerator biomeGenerator;
    private TerrainGenerator terrainGenerator;
    private ObjectGenerator objectGenerator;
    private Vector2 size;
    private Vector2 biomeChunkSize;
    private Vector2 terrainChunkSize;
    
    public static EnvironmentGeneratorBuilder Start => new();

    public EnvironmentGeneratorBuilder SetBiomeGenerator(BiomeGenerator biomeGenerator)
    {
        this.biomeGenerator = biomeGenerator;
        return this;
    }
    
    public EnvironmentGeneratorBuilder SetTerrainGenerator(TerrainGenerator terrainGenerator)
    {
        this.terrainGenerator = terrainGenerator;
        return this;
    }
    
    public EnvironmentGeneratorBuilder SetObjectGenerator(ObjectGenerator objectGenerator)
    {
        this.objectGenerator = objectGenerator;
        return this;
    }
    
    public EnvironmentGeneratorBuilder SetSize(Vector2 sizeInPixels)
    {
        this.size = sizeInPixels;
        return this;
    }

    public EnvironmentGeneratorBuilder SetBiomeChunkSize(Vector2 sizeInPixels)
    {
        this.biomeChunkSize = sizeInPixels;
        return this;
    }

    public EnvironmentGeneratorBuilder SetTerrainChunkSize(Vector2 sizeInPixels) {
        this.terrainChunkSize = sizeInPixels;
        return this;
    }

    public EnvironmentGenerator End()
    {
        return new EnvironmentGenerator(this.biomeGenerator, this.terrainGenerator, this.objectGenerator, this.size, this.biomeChunkSize, this.terrainChunkSize);
    }

    public EnvironmentGeneratorBuilder SetAllToDefault()
    {
        return this.SetBiomeGenerator(BiomeGeneratorBuilder.Start.SetAllToDefault().End())
            .SetTerrainGenerator(TerrainGeneratorBuilder.Start.SetAllToDefault().End())
            .SetObjectGenerator(ObjectGeneratorBuilder.Start.SetAllToDefault().End())
            .SetSize(new Vector2(10000, 10000))
            .SetBiomeChunkSize(new Vector2(100, 100))
            .SetTerrainChunkSize(new Vector2(50, 50));
    }
    
    private EnvironmentGeneratorBuilder() {}
}