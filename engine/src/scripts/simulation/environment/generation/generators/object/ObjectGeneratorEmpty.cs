
public class ObjectGeneratorEmpty : ObjectGenerator
{
    
    public override EnvironmentObjectData[] Generate(EnvironmentGenerationSettings settings, BiomeType[] biomeData, bool[] terrainData)
    {
        return new EnvironmentObjectData[] {};
    }

    public ObjectGeneratorEmpty() : base(0, 0, 0)
    {
        
    }
}