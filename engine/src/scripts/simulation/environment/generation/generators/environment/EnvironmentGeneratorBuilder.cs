
using Godot;

public class EnvironmentGeneratorBuilder
{
    private BiomeGenerator biomeGenerator;
    private TerrainGenerator terrainGenerator;
    private ObjectGenerator objectGenerator;
    private Vector2 size;
    private Vector2 biomeChunkSize;
    private Vector2 terrainChunkSize;
    private Vector2[] terrainPoints;
    private Vector2[] oceanPoints;
    private float oceanSizeMultiplier;

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

    public EnvironmentGeneratorBuilder SetTerrainChunkSize(Vector2 sizeInPixels)
    {
        this.terrainChunkSize = sizeInPixels;
        return this;
    }

    public EnvironmentGeneratorBuilder SetTerrainPoints(Vector2[] terrainPoints)
    {
        this.terrainPoints = terrainPoints;
        return this;
    }

    public EnvironmentGeneratorBuilder SetOceanPoints(Vector2[] oceanPoints)
    {
        this.oceanPoints = oceanPoints;
        return this;
    }

    public EnvironmentGeneratorBuilder SetOceanSizeMultiplier(float oceanSizeMultiplier)
    {
        this.oceanSizeMultiplier = oceanSizeMultiplier;
        return this;
    }

    public EnvironmentGenerator End()
    {
        return new EnvironmentGenerator(this.biomeGenerator, this.terrainGenerator, this.objectGenerator, this.size,
            this.biomeChunkSize, this.terrainChunkSize, this.terrainPoints, this.oceanPoints, this.oceanSizeMultiplier);
    }

    public EnvironmentGeneratorBuilder SetAllToDefault()
    {
        return this.SetBiomeGenerator(BiomeGeneratorBuilder.Start.SetAllToDefault().End())
            .SetTerrainGenerator(TerrainGeneratorBuilder.Start.SetAllToDefault().End())
            .SetObjectGenerator(ObjectGeneratorBuilder.Start.SetAllToDefault().End())
            .SetSize(new Vector2(10000, 10000))
            .SetBiomeChunkSize(new Vector2(100, 100))
            .SetTerrainChunkSize(new Vector2(50, 50))
            .SetTerrainPoints(new Vector2[]
            {
                new(0.5f, 0.5f)
            })
            .SetOceanPoints(new Vector2[] { })
            .SetOceanSizeMultiplier(1.0f);
    }

    private EnvironmentGeneratorBuilder() { }
}