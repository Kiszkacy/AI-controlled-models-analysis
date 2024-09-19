using System;
using System.Linq;

using Godot;

public partial class Environment : Node2D
{
    [Export]
    public Vector2 Size { get; set; } = new(1024, 1024);

    [Export(PropertyHint.Range, "1,15,or_greater")]
    public int TreeCount { get; set; } = 3;

    private PackedScene packedTree = ResourceLoader.Load<PackedScene>("res://src/scenes/environment/objects/trees/appleTree.tscn");

    public override void _Ready()
    {
        this.Generate();
    }

    private void Generate()
    {
        this.GenerateTrees();
    }

    private void GenerateTrees()
    {
        for (int index = 0; index < this.TreeCount; index++)
        {
            Node2D treeInstance = (Node2D)this.packedTree.Instantiate();
            Tree tree = (Tree)treeInstance;
            Vector2 spawnPosition;
            do
            {
                spawnPosition = new Vector2(
                    RandomGenerator.Float(0, 1) * this.Size.X,
                    RandomGenerator.Float(0, 1) * this.Size.Y
                );
            } while (this.GetChildren().Any(children => ((Tree)children).GlobalPosition.DistanceTo(spawnPosition) <= tree.SpawnSafeDistance*1.5f));
            tree.GlobalPosition = spawnPosition;
            this.AddChild(treeInstance);
        }
    }
}