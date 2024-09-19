using System;

using Godot;

public partial class Food : Area2D
{
    [Export(PropertyHint.Range, "1,100,or_greater")]
    public float EnergyNutrition { get; set; } = 30.0f;

    [Export(PropertyHint.Range, "5,100,or_greater")]
    public float Lifetime { get; set; } = 40.0f; // in sec

    private readonly Timer lifetimeTimer;

    private Sprite2D sprite;

    public override void _Ready()
    {
        this.sprite = this.GetNode<Sprite2D>("Sprite");
        this.lifetimeTimer.Activate(this.Lifetime);
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
        EntityManager.Get().RemoveFood(this);
        this.QueueFree();
    }

    public float Eat()
    {
        this.Die();
        return this.EnergyNutrition;
    }

    public Food()
    {
        this.lifetimeTimer = new(this.Die);
    }
}
