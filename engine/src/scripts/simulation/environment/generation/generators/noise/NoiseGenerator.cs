
using Godot;

public class NoiseGenerator
{
    private readonly FastNoiseLite noise;

    public FastNoiseLite.NoiseTypeEnum NoiseType => this.noise.NoiseType;
    public int Seed => this.noise.Seed;
    public float Frequency => this.noise.Frequency;
    public int Octaves => this.noise.FractalOctaves;
    public float Gain => this.noise.FractalGain;

    public float At(Vector2 position)
    {
        return this.noise.GetNoise2D(position.X, position.Y);
    }

    public float At(float positionX, float positionY)
    {
        return this.noise.GetNoise2D(positionX, positionY);
    }

    public NoiseGenerator(FastNoiseLite fastNoiseLite)
    {
        this.noise = fastNoiseLite;
    }
}