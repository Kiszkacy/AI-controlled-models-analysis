using System;
using Godot;

public partial class Tree : Node2D
{
	[Export(PropertyHint.Range, "0,25,or_greater")] 
	public float FoodPerMinute { get; set; } = 10;

	[Export(PropertyHint.Range, "64,512,or_greater")]
	public float FoodSpawnOuterRadius { get; set; } = 128;
	
	[Export(PropertyHint.Range, "0,64,or_greater")]
	public float FoodSpawnInnerRadius { get; set; } = 32;

	private Timer spawnFoodTimer;
	private PackedScene packedFood = ResourceLoader.Load<PackedScene>("res://src/scenes/food.tscn");

	public override void _Ready()
	{
		this.ResetTimer();
	}

	public override void _PhysicsProcess(double delta)
	{
		this.spawnFoodTimer.Process(delta);
	}

	private void SpawnFood()
	{
		Node2D foodInstance = (Node2D)this.packedFood.Instantiate();
		this.AddChild(foodInstance);
		Vector2 spawnOffset = new Vector2(
			(float)((new Random().NextDouble()-0.5f >= 0 ? 1 : -1) * (new Random().NextDouble() * (this.FoodSpawnOuterRadius-this.FoodSpawnInnerRadius) + this.FoodSpawnInnerRadius)),
			(float)((new Random().NextDouble()-0.5f >= 0 ? 1 : -1) * (new Random().NextDouble() * (this.FoodSpawnOuterRadius-this.FoodSpawnInnerRadius) + this.FoodSpawnInnerRadius))
		);
		foodInstance.GlobalPosition = this.GlobalPosition + spawnOffset;

		this.ResetTimer();
	}

	private void ResetTimer()
	{
		this.spawnFoodTimer.Activate(60.0f/this.FoodPerMinute);
	}

	public Tree()
	{
		this.spawnFoodTimer = new(this.SpawnFood);
	}
}
