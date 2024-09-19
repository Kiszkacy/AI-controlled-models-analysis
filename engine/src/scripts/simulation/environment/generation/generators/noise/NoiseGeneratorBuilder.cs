
using System;

using Godot;

public class NoiseGeneratorBuilder
{
    private readonly FastNoiseLite noise = new();

    public static NoiseGeneratorBuilder Start => new();

    public NoiseGeneratorBuilder SetNoiseType(FastNoiseLite.NoiseTypeEnum type)
    {
        this.noise.NoiseType = type;
        return this;
    }

    public NoiseGeneratorBuilder SetSeed(int seed)
    {
        this.noise.Seed = seed;
        return this;
    }

    public NoiseGeneratorBuilder SetFrequency(float frequency)
    {
        this.noise.Frequency = frequency;
        return this;
    }

    public NoiseGeneratorBuilder SetOctaves(int octaves)
    {
        this.noise.FractalOctaves = octaves;
        return this;
    }

    public NoiseGeneratorBuilder SetGain(float gain)
    {
        this.noise.FractalGain = gain;
        return this;
    }

    public NoiseGenerator End()
    {
        return new NoiseGenerator(this.noise);
    }

    public NoiseGeneratorBuilder SetAllToDefault()
    {
        return this.SetNoiseType(FastNoiseLite.NoiseTypeEnum.Perlin)
            .SetSeed(new Random().Next())
            .SetFrequency(0.5f)
            .SetOctaves(3)
            .SetGain(0.5f);
    }

    private NoiseGeneratorBuilder() { }
}