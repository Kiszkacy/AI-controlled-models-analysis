using Godot;

public class ObjectGeneratorBuilder
{
    private int maxAmountOfSpawnTries;
    private int safeSpaceDistance;

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

    public ObjectGenerator End()
    {
        return new ObjectGenerator(this.maxAmountOfSpawnTries, this.safeSpaceDistance);
    }

    public ObjectGeneratorBuilder SetAllToDefault()
    {
        return this.SetMaxAmountOfSpawnTries(10)
            .SetSafeSpaceDistance(16);
    }

    private ObjectGeneratorBuilder() { }
}