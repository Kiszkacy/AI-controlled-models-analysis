using System;
using Godot;

public partial class Food : Node2D
{
	[Export] 
	public float EnergyNutrition { get; set; } = 5;

	public override void _Ready()
	{
		Sprite2D sprite = this.GetNode<Sprite2D>("Sprite");
		sprite.Modulate = Color.FromHsv((float)(new Random().NextDouble() * 0.1f), (float)(new Random().NextDouble() * 0.5f + 0.5f), 1.0f);;
	}
}
