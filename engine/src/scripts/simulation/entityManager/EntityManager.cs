
using System;
using System.Collections.Generic;

using Godot;
using Godot.Collections;

public class EntityManager : Singleton<EntityManager>, Initializable
{
    public EntityLayer<Food> FoodBuckets { get; private set; }
    public EntityLayer<EnvironmentObject> ObjectBuckets { get; private set; }

    private readonly InitializableWrapper initialized = new();
    public bool IsInitialized => this.initialized.IsInitialized;

    public void Initialize(Vector2 environmentSize)
    {
        this.FoodBuckets = new EntityLayer<Food>(environmentSize);
        this.FoodBuckets.Initialize();

        this.ObjectBuckets = new EntityLayer<EnvironmentObject>(environmentSize);
        this.ObjectBuckets.Initialize();

        this.initialized.Initialize();
    }

    public void Reset()
    {
        this.FoodBuckets.Reset();
        this.FoodBuckets = null;
        this.ObjectBuckets.Reset();
        this.ObjectBuckets = null;
        this.initialized.Reset();
    }

    public FoodSpawnerSaveData[] SaveFoodSpawners()
    {
        var foodSpawnersData = new List<FoodSpawnerSaveData>();
        this.ObjectBuckets.ForEachEntity(entity =>
        {
            if (entity is Tree or Bush)
            {
                if (entity.FindChild("FoodSpawner") is FoodSpawner foodSpawner)
                {
                    foodSpawnersData.Add(foodSpawner.Save());
                }
            }
        });
        return foodSpawnersData.ToArray();
    }

    public void LoadFoodSpawnersData(FoodSpawnerSaveData[] foodSpawnerSaveData, FoodSaveData[] foodSaveData)
    {
        this.ObjectBuckets.ForEachEntity(entity =>
        {
            if (entity is Tree or Bush)
            {
                if (entity.FindChild("FoodSpawner") is FoodSpawner foodSpawner)
                {
                    foreach (FoodSpawnerSaveData saveData in foodSpawnerSaveData)
                    {
                        if (saveData.Position == entity.GlobalPosition)
                        {
                            foodSpawner.Load(saveData);
                        }
                    }

                    foreach (FoodSaveData foodData in foodSaveData)
                    {
                        if (foodData.SpawnerPosition == entity.GlobalPosition)
                        {
                            foodSpawner.AddFood(foodData.Position, foodData.TimeLeft);
                        }
                    }
                }
            }
        });
    }

    public FoodSaveData[] SaveFood()
    {
        var foodData = new List<FoodSaveData>();
        this.FoodBuckets.ForEachEntity(entity =>
        {
            foodData.Add(entity.Save());
        });
        return foodData.ToArray();
    }
}