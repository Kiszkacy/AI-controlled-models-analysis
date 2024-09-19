using Godot;

public class ObjectGeneratorBuilder
{
    public static ObjectGeneratorBuilder Start => new();

    public ObjectGenerator End()
    {
        return new ObjectGenerator();
    }

    public ObjectGeneratorBuilder SetAllToDefault()
    {
        return this;
    }
    
    private ObjectGeneratorBuilder() {}
}