
using System;

using Godot;

public partial class FoodSpawner : Node, Initializable // TODO remove exports, node inheritance and made it into a normal script if performance is bad
{
    [Export(PropertyHint.Range, "0,25,or_greater")]
    public float FoodPerMinute { get; set; } = 4;

    [Export(PropertyHint.Range, "64,512,or_greater")]
    public float FoodSpawnOuterRadius { get; set; } = 256;

    [Export(PropertyHint.Range, "0,128,or_greater")]
    public float FoodSpawnInnerRadius { get; set; } = 64;

    [Export(PropertyHint.Range, "1,32,or_greater")]
    public int MaxFoodCount { get; set; } = 8;

    [Export(PropertyHint.Range, "0,32,or_greater")]
    public int InitialFoodCount { get; set; } = 1;

    [Export(PropertyHint.Range, "1,100,or_greater")]
    public float FoodEnergyNutrition { get; set; } = 30.0f;

    [Export]
    public Node2D SpawnPositionTarget { get; set; }

    [Export]
    public bool InitializeAutomatically { get; set; } = true;

    private readonly Timer spawnFoodTimer;
    private PackedScene packedFood = ResourceLoader.Load<PackedScene>("res://src/scenes/simulation/food/food.tscn");

    private const int maxSpawnTryCount = 5;

    private readonly InitializableWrapper initialized = new();
    public bool IsInitialized => this.initialized.IsInitialized;

    public override void _Ready()
    {
        if (this.InitializeAutomatically)
        {
            this.Initialize();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!this.spawnFoodTimer.IsActive && !this.IsFull)
        {
            this.spawnFoodTimer.Activate(60.0f/this.FoodPerMinute);
        }

        this.spawnFoodTimer.Process(delta);
    }

    public void Initialize()
    {
        if (!Reloader.Instance.IsReloading)
        {
            for (int i = 0; i < this.InitialFoodCount; i++) this.SpawnFood();
            this.ResetTimer();
        }

        this.initialized.Initialize();
    }

    private void SpawnFood()
    {
        Node2D foodInstance = (Node2D)this.packedFood.Instantiate();
        this.AddChild(foodInstance);

        int tryCount = 0;
        bool validSpawnPosition = false;
        Vector2 spawnPosition = Vector2.Zero;
        while (!validSpawnPosition && tryCount < maxSpawnTryCount)
        {
            tryCount += 1;

            spawnPosition = this.SpawnPositionTarget.GlobalPosition + new Vector2(
                (RandomGenerator.Occurs(0.5f) ? 1 : -1) *
                RandomGenerator.Float(this.FoodSpawnInnerRadius, this.FoodSpawnOuterRadius),
                (RandomGenerator.Occurs(0.5f) ? 1 : -1) *
                RandomGenerator.Float(this.FoodSpawnInnerRadius, this.FoodSpawnOuterRadius)
            );

            validSpawnPosition = EnvironmentManager.Instance.IsTerrainAt(spawnPosition);
        }

        if (!validSpawnPosition)
        {
            return;
        }

        foodInstance.GlobalPosition = spawnPosition;
        Food food = (Food)foodInstance;
        EntityManager.Get().FoodBuckets.RegisterEntity(food);

        if (!this.IsFull)
        {
            this.ResetTimer();
        }
    }

    public void AddFood(Vector2 position, double timeLeft)
    {
        Node2D foodInstance = (Node2D)this.packedFood.Instantiate();
        this.AddChild(foodInstance);
        foodInstance.GlobalPosition = position;
        Food food = (Food)foodInstance;
        food.SetTimeLeft(timeLeft);
        EntityManager.Get().FoodBuckets.RegisterEntity(food);
    }

    private void ResetTimer()
    {
        this.spawnFoodTimer.Activate(60.0f/this.FoodPerMinute);
    }

    private bool IsFull => this.GetChildren().Count >= this.MaxFoodCount;

    public FoodSpawnerSaveData Save()
    {
        return new FoodSpawnerSaveData(this.spawnFoodTimer.Time, spawnFoodTimer.IsActive, this.SpawnPositionTarget.GlobalPosition);
    }

    public void Load(FoodSpawnerSaveData saveData)
    {
        this.spawnFoodTimer.Time = saveData.TimerTime;
        this.spawnFoodTimer.IsActive = saveData.TimerIsActive;
    }

    public string GetStats()
    {
        return $"Food Spawner:\n" +
               $"- Timer Active: {this.spawnFoodTimer.IsActive}\n" +
               $"- Time Left: {this.spawnFoodTimer.Time:F2} seconds\n" +
               $"- Food Energy Nutrition: {this.FoodEnergyNutrition}\n" +
               $"- Food Per Minute: {this.FoodPerMinute}";
    }
    public FoodSpawner()
    {
        this.spawnFoodTimer = new(this.SpawnFood);
    }
}