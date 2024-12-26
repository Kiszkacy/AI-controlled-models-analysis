using System;

public class ObjectGeneratorBuilder
{
    private int maxAmountOfSpawnTries;
    private int safeSpaceDistance;
    private float objectDensityMultiplier;
    private ObjectGeneratorType generatorType;

    public static ObjectGeneratorBuilder Start => new();

    public ObjectGeneratorBuilder SetMaxAmountOfSpawnTries(int maxAmountOfSpawnTries)
    {
        this.maxAmountOfSpawnTries = maxAmountOfSpawnTries;
        return this;
    }

    public ObjectGeneratorBuilder SetSafeSpaceDistance(int safeSpaceDistance)
    {
        this.safeSpaceDistance = safeSpaceDistance;
        return this;
    }

    public ObjectGeneratorBuilder SetObjectDensityMultiplier(float objectDensityMultiplier)
    {
        this.objectDensityMultiplier = objectDensityMultiplier;
        return this;
    }

    public ObjectGeneratorBuilder SetGeneratorType(ObjectGeneratorType type)
    {
        this.generatorType = type;
        return this;
    }

    public ObjectGenerator End()
    {
        return this.generatorType switch
        {
            ObjectGeneratorType.Default => new ObjectGenerator(this.maxAmountOfSpawnTries, this.safeSpaceDistance, this.objectDensityMultiplier),
            ObjectGeneratorType.Empty => new ObjectGeneratorEmpty(),
            _ => throw new Exception()
        };
    }

    public ObjectGeneratorBuilder SetAllToDefault()
    {
        return this.SetMaxAmountOfSpawnTries(10)
            .SetSafeSpaceDistance(16)
            .SetObjectDensityMultiplier(1.0f);
    }

    private ObjectGeneratorBuilder() { }
}