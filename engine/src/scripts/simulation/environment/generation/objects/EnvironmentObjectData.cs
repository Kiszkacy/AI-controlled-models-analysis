
using Godot;

public readonly struct EnvironmentObjectData
{
    public readonly EnvironmentObjectId Id;
    public readonly Vector2 Position;

    public EnvironmentObjectData(EnvironmentObjectId id, Vector2 position)
    {
        this.Id = id;
        this.Position = position;
    }
}