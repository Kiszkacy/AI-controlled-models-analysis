
using Godot;

public abstract partial class EnvironmentObject : Node2D, Bucketable, Trackable
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

    public EnvironmentObject(float initialScale)
    {
        this.InitialScale = Mathf.Clamp(initialScale, 0.5f, 1.5f);
    }

    public EnvironmentObject()
    {

    }
    
    public virtual string[] GetInformation()
    {
        return new[] { "EnvironmentObject" };
    }
}