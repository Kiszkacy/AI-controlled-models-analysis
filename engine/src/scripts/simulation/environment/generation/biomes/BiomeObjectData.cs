
public readonly struct BiomeObjectData
{
    public float AmountOfObjectsPer1000x1000Pixels { get; }
    public EnvironmentObjectId[] Objects { get; }
    public int[] Weights { get; }

    public BiomeObjectData(float amountOfObjectsPer1000X1000Pixels, EnvironmentObjectId[] objects, int[] weights)
    {
        this.AmountOfObjectsPer1000x1000Pixels = amountOfObjectsPer1000X1000Pixels;
        this.Objects = objects;
        this.Weights = weights;
    }
}