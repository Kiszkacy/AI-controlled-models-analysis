
using Godot;

public struct FoodSaveData
{
    public Vector2 Position { get; set; }
    public Vector2 SpawnerPosition { get; set; }
    public double TimeLeft { get; set; }

    public FoodSaveData(Vector2 position, Vector2 spawnerPosition, double timeLeft)
    {
        this.Position = position;
        this.SpawnerPosition = spawnerPosition;
        this.TimeLeft = timeLeft;
    }
}