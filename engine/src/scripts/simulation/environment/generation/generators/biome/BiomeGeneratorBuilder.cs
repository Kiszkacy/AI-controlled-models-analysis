using Godot;

public class BiomeGeneratorBuilder
{
    private NoiseGenerator noise1;
    private NoiseGenerator noise2;
    private float distanceRandomness;

    public static BiomeGeneratorBuilder Start => new();

    public BiomeGeneratorBuilder SetNoise1(NoiseGenerator noise)
    {
        this.noise1 = noise;
        return this;
    }

    public BiomeGeneratorBuilder SetNoise2(NoiseGenerator noise)
    {
        this.noise2 = noise;
        return this;
    }

    public BiomeGeneratorBuilder SetDistanceRandomness(float distanceRandomness)
    {
        this.distanceRandomness = distanceRandomness;
        return this;
    }

    public BiomeGenerator End()
    {
        return new BiomeGenerator(noise1, noise2, distanceRandomness);
    }

    public BiomeGeneratorBuilder SetAllToDefault()
    {
        return this.SetNoise1(NoiseGeneratorBuilder.Start.SetAllToDefault().SetFrequency(0.4f).End())
            .SetNoise2(NoiseGeneratorBuilder.Start.SetAllToDefault().End())
            .SetDistanceRandomness(0.025f);
    }

    private BiomeGeneratorBuilder() { }
}