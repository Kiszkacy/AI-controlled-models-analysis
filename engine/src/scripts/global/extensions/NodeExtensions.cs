using System.Collections.Generic;

using Godot;

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