
using Godot;

public struct FoodSaveData
{
    public Vector2 Position { get; set; }
    public Vector2 SpawnerPosition { get; set; }
    // TODO add lifetime left

    public FoodSaveData(Vector2 position, Vector2 spawnerPosition)
    {
        this.Position = position;
        this.SpawnerPosition = spawnerPosition;
    }
}