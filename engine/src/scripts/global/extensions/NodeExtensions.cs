using Godot;
using System.Collections.Generic;

public static class NodeExtensions
{
    public static void RemoveAllChildren(this Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            child.QueueFree();
        }
    }
}
