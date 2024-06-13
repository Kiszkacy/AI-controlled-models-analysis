using System;
using Godot;

public partial class Food : Area2D
{
    [Export(PropertyHint.Range, "1,100,or_greater")] 
    public float EnergyNutrition { get; set; } = 30.0f;
    
    [Export(PropertyHint.Range, "5,100,or_greater")] 
    public float Lifetime { get; set; } = 40.0f; // in sec

    private Timer lifetimeTimer;
    
    public override void _Ready()
    {
        Sprite2D sprite = this.GetNode<Sprite2D>("Sprite");
        sprite.Modulate = Color.FromHsv((float)(new Random().NextDouble() * 0.1f), (float)(new Random().NextDouble() * 0.5f + 0.5f), 1.0f, 0.75f);
        
        this.lifetimeTimer.Activate(this.Lifetime);
    }

    public override void _PhysicsProcess(double delta)
    {
        this.lifetimeTimer.Process(delta);
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
