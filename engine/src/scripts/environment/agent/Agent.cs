using Godot;
using System;

public partial class Agent : CharacterBody2D
{
	[Export]
	public float MaximumSpeed { get; set; } = 500.0f;
	
	[Export]
	public float MaximumEnergy { get; set; } = 200.0f;
	
	[Export]
	public float InitialEnergy { get; set; } = 50.0f;
	
	[Export]
	public float MaximumHealth { get; set; } = 200.0f;
	
	[Export]
	public float InitialHealth { get; set; } = 100.0f;
	
	[Export]
	public float MaximumTurnSpeedRadiansPerSecond { get; set; } = 6.0f;
	
	[Export]
	public float SightAngleRadians { get; set; } = 2.0f;
		
	private float energy;
	private float health;
	private float rotateRadiansPerSecond = 0.0f;
	
	public float getPointingAngle()
	{
		return Velocity.Angle();
	}
	
	private void UpdateEnergy(double delta)
	{
		this.energy -= Config.Instance.Data.Environment.EnergyLossPerSecond * (float)delta;
	}
	
	private void UpdateHealth(double delta)
	{
		this.health -= Config.Instance.Data.Environment.HealthLossPerSecond * (float)delta;
	}
	
	public override void _Ready()
	{
		this.energy = this.InitialEnergy;
		this.health = this.InitialHealth;
	}
	
	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if (this.energy > 0.0f)
		{
			this.UpdateEnergy(delta);
		}
		else 
		{
			this.UpdateHealth(delta);
		}
		this.GlobalRotation += (float) delta * rotateRadiansPerSecond;
		this.Velocity = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation)) * Velocity.Length();
		MoveAndCollide(Velocity);
	}
}
