
using Godot;

public partial class EnvironmentObject : Node2D, Bucketable
{
    [Export(PropertyHint.Range, "0.5,1.5,")]
    public float InitialScale { get; set; } = 1.0f;

    [Export(PropertyHint.Range, "16,512,")]
    public int SpawnSafeDistance { get; set; } = 64;

    public Vector2I BucketId { get; set; }

    public override void _Ready()
    {
        this.Scale = new Vector2(this.InitialScale, this.InitialScale);
    }

    public string GetStats()
    {
        string typeName = this is Tree ? "Tree" : this is Bush ? "Bush" : "Rock";
        string stats = $"{typeName}\n";

        if (this.HasNode("FoodSpawner") && this.GetNode("FoodSpawner") is FoodSpawner spawner)
        {
            stats += spawner.GetStats();
        }

        return stats;
    }

    public EnvironmentObject(float initialScale)
    {
        this.InitialScale = Mathf.Clamp(initialScale, 0.5f, 1.5f);
    }

    public EnvironmentObject()
    {

    }
}