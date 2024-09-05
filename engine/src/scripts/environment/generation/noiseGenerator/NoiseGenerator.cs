
using Godot;

public class NoiseGenerator
{
    private readonly FastNoiseLite noise;

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