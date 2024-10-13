
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
    
    public Dictionary ToDictionary()
    {
        var dict = new Dictionary
        {
            { "Id", (int)this.Id },  
            { "Position", this.Position }  
        };

        return dict;
    }
    
    public static EnvironmentObjectData FromDictionary(Dictionary data)
    {
        EnvironmentObjectId id = (EnvironmentObjectId)(int)data["Id"];
        Vector2 position = (Vector2)data["Position"];
        
        return new EnvironmentObjectData(id, position);
    }
}