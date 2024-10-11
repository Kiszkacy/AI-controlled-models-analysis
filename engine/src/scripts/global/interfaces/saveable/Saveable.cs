using Godot.Collections;

public interface Saveable
{
    public void Save(Dictionary data);
    public void Load(Dictionary data);
}