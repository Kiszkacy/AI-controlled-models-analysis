using Godot;
using System;
using System.Collections.Generic;

public partial class Agent : CharacterBody2D
{
	[Export]
	public float MaximumSpeed { get; set; } = 200.0f; // in px
	
	[Export]
	public float MaximumEnergy { get; set; } = 200.0f;
	
	[Export]
	public float InitialEnergy { get; set; } = 50.0f;
	
	[Export]
	public float MaximumHealth { get; set; } = 100.0f;
	
	[Export]
	public float InitialHealth { get; set; } = 100.0f;
	
	[Export]
	public float MaximumTurnSpeedRadiansPerSecond { get; set; } = 6.0f;
	
	[Export]
	public float SightAngleRadians { get; set; } = 2.0f;
	
	[Export]
	public float SightRadius { get; set; } = 256.0f; // in px
		
	private float energy;
	private float health;
	private float rotateRadiansPerSecond = 0.0f;
	
	public float GetPointingAngle() => this.Velocity.Angle();

	private void UpdateEnergy(double delta)
	{
		this.energy -= Config.Instance.Data.Environment.EnergyLossPerSecond * (float)delta;
	}
	
	private void UpdateHealth(double delta)
	{
		if (this.energy <= 0.0f) // TODO proper float comparison
		{
			this.health -= Config.Instance.Data.Environment.HealthLossPerSecond * (float)delta;
		}
		else
		{
			this.health += Config.Instance.Data.Environment.HealthRegenPerSecond * (float)delta;
		}
	}

	private void MovementProcess(double delta)
	{
		this.GlobalRotation += (float) delta * rotateRadiansPerSecond;
		float angle = Math.GodotToRad(this.GlobalRotation);
		this.Velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Velocity.Length();
		this.MoveAndCollide(Velocity);
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
		this.UpdateEnergy(delta);
		this.UpdateHealth(delta);
		this.MovementProcess(delta);
	}
}
