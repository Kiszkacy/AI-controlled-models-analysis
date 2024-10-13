


using System;
using System.Linq;

using Godot;
using Godot.Collections;

using Array = Godot.Collections.Array;

public struct EnvironmentTemplate
{
    public EnvironmentGenerationSettings GenerationSettings { get; set; }
    public BiomeType[] BiomeData { get; set;  }
    public bool[] TerrainData { get; set;  }
    public EnvironmentObjectData[] ObjectData { get; set;  }

    private void _Instantiate(Environment environment, bool shouldInitialize)
    {
        if (shouldInitialize)
        {
            environment.Initialize(this, true);
        }
        Node objectNode = environment.GetNode("Objects");

        foreach (EnvironmentObjectData objectData in this.ObjectData)
        {
            PackedScene packedObject = EnvironmentObjectTable.Get(objectData.Id);
            EnvironmentObject instantiatedObject = packedObject.Instantiate<EnvironmentObject>();
            instantiatedObject.GlobalPosition = objectData.Position;

            if (!this.CanObjectBePlaced(instantiatedObject))
            {
                continue;
            }

            objectNode.AddChild(instantiatedObject);

            EntityManager.Get().ObjectBuckets.RegisterEntity(instantiatedObject);
        }
    }

    public Environment Instantiate()
    {
        PackedScene packedEnvironment = ResourceLoader.Load<PackedScene>("res://src/scenes/simulation/environment/environment.tscn");
        Environment environment = packedEnvironment.Instantiate<Environment>();

        this._Instantiate(environment, true);
        return environment;
    }

    public void InstantiateInto(Environment environment)
    {
        this._Instantiate(environment, false);
    }

    private bool CanObjectBePlaced(EnvironmentObject object_) // TODO this could be optimized in a way that object is instantiated after this check not before
    {
        Vector2I objectBucketId = EntityManager.Instance.ObjectBuckets.VectorToBucketId(object_.GlobalPosition);
        EnvironmentObject[] neighbors = EntityManager.Instance.ObjectBuckets.GetEntitiesFrom3x3(objectBucketId);

        foreach (EnvironmentObject neighbor in neighbors)
        {
            float requiredDistance = Mathf.Max(neighbor.SpawnSafeDistance, object_.SpawnSafeDistance);
            if (neighbor.GlobalPosition.DistanceTo(object_.GlobalPosition) < requiredDistance)
            {
                return false;
            }
        }

        return true;
    }

    public EnvironmentTemplate(EnvironmentGenerationSettings generationSettings, BiomeType[] biomeData, bool[] terrainData, EnvironmentObjectData[] objectData)
    {
        this.GenerationSettings = generationSettings;
        this.BiomeData = biomeData;
        this.TerrainData = terrainData;
        this.ObjectData = objectData;
    }
}