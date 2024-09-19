
using System.Collections.Generic;

using Godot;

public static class EnvironmentObjectTable
{
    private static readonly Dictionary<EnvironmentObjectId, PackedScene> Table = new()
    {
        // TREES
        {
            EnvironmentObjectId.SmallTree,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/trees/smallTree.tscn")
        },
        {
            EnvironmentObjectId.OakTree,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/trees/oakTree.tscn")
        },
        {
            EnvironmentObjectId.AppleTree,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/trees/appleTree.tscn")
        },
        {
            EnvironmentObjectId.PineTree,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/trees/pineTree.tscn")
        },
        {
            EnvironmentObjectId.LargePineTree,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/trees/largePineTree.tscn")
        },
        
        // BUSHES
        {
            EnvironmentObjectId.BerryBush,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/bushes/berryBush.tscn")
        },
        {
            EnvironmentObjectId.SmallBush,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/bushes/smallBush.tscn")
        },
        {
            EnvironmentObjectId.BigBush,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/bushes/bigBush.tscn")
        },
        
        // ROCKS
        {
            EnvironmentObjectId.MediumRock,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/rocks/mediumRock.tscn")
        },
        {
            EnvironmentObjectId.LargeRock,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/rocks/largeRock.tscn")
        },
        {
            EnvironmentObjectId.GiantRock,
            ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/rocks/giantRock.tscn")
        }
    };

    public static PackedScene Get(EnvironmentObjectId id) => Table[id];

}