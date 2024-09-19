
public class TerrainGeneratorBuilder
{
    private int passCount;
    private int minimalRequiredOceanNeighborCountToCorrode;
    private float corrosionChance;

    public static TerrainGeneratorBuilder Start => new();

    public TerrainGeneratorBuilder SetPassCount(int passCount)
    {
        this.passCount = passCount;
        return this;
    }

    public TerrainGeneratorBuilder SetMinimalRequiredOceanNeighborCountToCorrode(int minimalRequiredOceanNeighborCountToCorrode)
    {
        this.minimalRequiredOceanNeighborCountToCorrode = minimalRequiredOceanNeighborCountToCorrode;
        return this;
    }

    public TerrainGeneratorBuilder SetCorrosionChance(float corrosionChance)
    {
        this.corrosionChance = corrosionChance;
        return this;
    }

    public TerrainGenerator End()
    {
        return new TerrainGenerator(this.passCount, this.minimalRequiredOceanNeighborCountToCorrode, this.corrosionChance);
    }

    public TerrainGeneratorBuilder SetAllToDefault()
    {
        return this.SetPassCount(1)
            .SetMinimalRequiredOceanNeighborCountToCorrode(3)
            .SetCorrosionChance(0.5f);
    }

    private TerrainGeneratorBuilder() { }
}