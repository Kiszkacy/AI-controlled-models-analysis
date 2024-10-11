
using Godot;
using Godot.Collections;

public readonly struct EnvironmentObjectData
{
    public readonly EnvironmentObjectId Id;
    public readonly Vector2 Position;

    public EnvironmentObjectData(EnvironmentObjectId id, Vector2 position)
    {
        this.Id = id;
        this.Position = position;
    }
    
    public Dictionary ToDictionary()
    {
        var dict = new Dictionary
        {
            { "Id", (int)this.Id },  
            { "Position", this.Position }  
        };

        return dict;
    }
}