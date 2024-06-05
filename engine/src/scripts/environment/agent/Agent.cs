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
	public float SightAngleInRadians { get; set; } = Mathf.Pi / 4.0f;
	
	[Export]
	public float SightRadius { get; set; } = 256.0f; // in px
		
	private float energy;
	private float health;
	private float rotateRadiansPerSecond = 0.0f;

	public Vector2 Direction { get; set; } = Vector2.Right;
	public float DirectionAngle => this.Direction.Angle();

	private Food closestFood = null;
	public float DistanceToClosestFood => this.closestFood?.GlobalPosition.DistanceTo(this.GlobalPosition) ?? float.NaN;
	public float AngleToClosestFood => this.closestFood != null ? this.Direction.AngleTo(this.closestFood.GlobalPosition - this.GlobalPosition) : float.NaN;

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

	private void SightProcess()
	{
		this.closestFood = null;
		
		foreach (Food food in EntityManager.Get().Food)
		{
			float distanceToFood = food.GlobalPosition.DistanceTo(this.GlobalPosition);
			if (distanceToFood >= this.SightRadius)
			{
				return;
			}

			if (this.closestFood != null && this.closestFood.GlobalPosition.DistanceTo(this.GlobalPosition) <= distanceToFood)
			{
				return;
			}

			Vector2 directionToFood = (food.GlobalPosition - this.GlobalPosition).Normalized();
			float angleToFoodInRadians = this.Direction.AngleTo(directionToFood);
			if (Mathf.Abs(angleToFoodInRadians) < this.SightAngleInRadians / 2.0f)
			{
				this.closestFood = food;
			}
		}
	}

	private void onMouthBodyEntered(Node2D body)
	{
		if (body is not Food food) return;
		
		float nutrition = food.Eat();
		this.energy = Mathf.Clamp(this.energy + nutrition, 0.0f, this.MaximumEnergy);
	}
	
	private void MovementProcess(double delta)
	{
		this.GlobalRotation += (float)delta * this.rotateRadiansPerSecond;
		float angle = Math.GodotToRad(this.GlobalRotation);
		this.Velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * this.Velocity.Length();
		this.MoveAndCollide(this.Velocity);
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
		this.SightProcess();
	}
}
