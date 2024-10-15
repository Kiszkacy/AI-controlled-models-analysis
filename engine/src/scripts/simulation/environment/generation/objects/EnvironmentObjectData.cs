
using Godot;
using Godot.Collections;

public struct EnvironmentObjectData
{
    public EnvironmentObjectId Id { get; set; }
    public Vector2 Position { get; set; }

    public EnvironmentObjectData(EnvironmentObjectId id, Vector2 position)
    {
        this.Id = id;
        this.Position = position;
    }
}