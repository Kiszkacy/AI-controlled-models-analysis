using Godot;

public class TerrainGeneratorBuilder
{
    public static TerrainGeneratorBuilder Start => new();

    public TerrainGenerator End()
    {
        return new TerrainGenerator();
    }

    public TerrainGeneratorBuilder SetAllToDefault()
    {
        return this;
    }
    
    private TerrainGeneratorBuilder() {}
}