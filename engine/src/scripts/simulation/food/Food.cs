
using System.IO;

using Godot;

public partial class Food : Area2D, Bucketable, Trackable
{
    [Export(PropertyHint.Range, "1,100,or_greater")]
    public float EnergyNutrition { get; set; } = 30.0f;

    [Export(PropertyHint.Range, "5,100,or_greater")]
    public float Lifetime { get; set; } = 40.0f; // in sec

    public Vector2I BucketId { get; set; }

    private readonly Timer lifetimeTimer;
    private Sprite2D sprite;

    public override void _Ready()
    {
        this.sprite = this.GetNode<Sprite2D>("Sprite");
        if (!Reloader.Instance.IsReloading)
        {
            this.lifetimeTimer.Activate(this.Lifetime);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        this.lifetimeTimer.Process(delta);
        this.UpdateColor();
    }

    private void UpdateColor()
    {
        float ratio = (float)this.lifetimeTimer.Time / this.Lifetime;
        this.sprite.Modulate = new Color(ratio, 0.0f, 1.0f-ratio);
    }

    private void Die()
    {
        EntityManager.Get().FoodBuckets.RemoveEntity(this);
        this.QueueFree();
    }

    public float Eat()
    {
        this.Die();
        return this.EnergyNutrition;
    }

    public void SetTimeLeft(double time)
    {
        this.lifetimeTimer.Activate(time);
    }

    public FoodSaveData Save()
    {
        FoodSpawner parent = (FoodSpawner)this.GetParent();
        return new FoodSaveData(this.GlobalPosition, parent.SpawnPositionTarget.GlobalPosition, this.lifetimeTimer.Time);
    }

    public Food()
    {
        this.lifetimeTimer = new(this.Die);
    }

    public string[] GetInformation()
    {
        return new[]
        {
            $"Position: ({this.GlobalPosition.X:F2}; {this.GlobalPosition.Y:F2})",
            $"Will disappear in: {this.lifetimeTimer.Time:F2} seconds",
            $"Nutrition: {this.EnergyNutrition}",
        };
    }
}