
using System;
using System.Collections.Generic;

using Godot;

public readonly struct BiomeTableRow
{
    public float DistanceFromCenterStart { get; }
    public float DistanceFromCenterEnd { get; }
    public BiomeType[][] Biomes { get; }

    public BiomeTableRow(float distanceFromCenterStart, float distanceFromCenterEnd, BiomeType[][] biomes)
    {
        this.DistanceFromCenterStart = distanceFromCenterStart;
        this.DistanceFromCenterEnd = distanceFromCenterEnd;
        this.Biomes = biomes;
    }
}

public static class BiomeTable
{
    public static BiomeTableRow[] Biomes =
    {
        new(0.9f, 1.0f, new[]
        {
            new[] { BiomeType.Ocean }
        }),
        new(0.8f, 0.9f, new[]
        {
            new[] { BiomeType.Ocean },
            new[] { BiomeType.Beach,               BiomeType.Ocean,               BiomeType.Ocean },
            new[] { BiomeType.Beach,               BiomeType.Beach,               BiomeType.Ocean,               BiomeType.Ocean,               BiomeType.Ocean },
            new[] { BiomeType.Beach,               BiomeType.Beach,               BiomeType.Beach,               BiomeType.Ocean,               BiomeType.Ocean },
            new[] { BiomeType.Beach,               BiomeType.Beach,               BiomeType.Beach,               BiomeType.Beach,               BiomeType.Ocean }
        }),
        new(0.6f, 0.8f, new[]
        {
            new[] { BiomeType.OakForest,           BiomeType.OakForest,           BiomeType.OakForest,           BiomeType.OakForestOutskirts,  BiomeType.Flatland },
            new[] { BiomeType.OakForest,           BiomeType.OakForest,           BiomeType.OakForest,           BiomeType.OakForestOutskirts,  BiomeType.Flatland },
            new[] { BiomeType.OakForestOutskirts,  BiomeType.BushFlatland,        BiomeType.BushFlatland,        BiomeType.Flatland,            BiomeType.Flatland },
            new[] { BiomeType.OakForestOutskirts,  BiomeType.Flatland,            BiomeType.Flatland,            BiomeType.Flatland,            BiomeType.Flatland },
            new[] { BiomeType.RockyFlatland,       BiomeType.RockyFlatland,       BiomeType.RockylandOutskirts,  BiomeType.RockylandOutskirts,  BiomeType.RockylandOutskirts }
        }),
        new(0.4f, 0.6f, new[]
        {
            new[] { BiomeType.PineForestOutskirts, BiomeType.OakForest,           BiomeType.OakForest,           BiomeType.DenseBushFlatland,   BiomeType.Flatland },
            new[] { BiomeType.OakForest,           BiomeType.OakForest,           BiomeType.DenseBushFlatland,   BiomeType.OakForestOutskirts,  BiomeType.Flatland },
            new[] { BiomeType.OakForestOutskirts,  BiomeType.BushFlatland,        BiomeType.BushFlatland,        BiomeType.OakForest,           BiomeType.Flatland },
            new[] { BiomeType.BushFlatland,        BiomeType.Flatland,            BiomeType.Flatland,            BiomeType.OakForest,           BiomeType.Flatland },
            new[] { BiomeType.RockyFlatland,       BiomeType.RockyFlatland,       BiomeType.RockylandOutskirts,  BiomeType.RockylandOutskirts,  BiomeType.RockylandOutskirts }
        }),
        new(0.2f, 0.4f, new[]
        {
            new[] { BiomeType.PineForest,          BiomeType.PineForestOutskirts, BiomeType.OakForest,           BiomeType.DenseBushFlatland,   BiomeType.Flatland },
            new[] { BiomeType.PineForest,          BiomeType.PineForestOutskirts, BiomeType.BushFlatland,        BiomeType.BushFlatland,        BiomeType.Flatland },
            new[] { BiomeType.PineForestOutskirts, BiomeType.BushFlatland,        BiomeType.BushFlatland,        BiomeType.Flatland,            BiomeType.Flatland },
            new[] { BiomeType.PineForestOutskirts, BiomeType.OakForest,           BiomeType.Flatland,            BiomeType.Flatland,            BiomeType.RockyFlatland },
            new[] { BiomeType.Rockyland,           BiomeType.RockyFlatland,       BiomeType.RockylandOutskirts,  BiomeType.RockylandOutskirts,  BiomeType.RockylandOutskirts }
        }),
        new(0.0f, 0.2f, new[]
        {
            new[] { BiomeType.DensePineForest,     BiomeType.DensePineForest,     BiomeType.OakForest,           BiomeType.PineForest,          BiomeType.PineForestOutskirts },
            new[] { BiomeType.DensePineForest,     BiomeType.DensePineForest,     BiomeType.OakForest,           BiomeType.PineForest,          BiomeType.PineForestOutskirts },
            new[] { BiomeType.DensePineForest,     BiomeType.OakForest,           BiomeType.DensePineForest,     BiomeType.PineForestOutskirts, BiomeType.Flatland },
            new[] { BiomeType.DensePineForest,     BiomeType.OakForest,           BiomeType.DensePineForest,     BiomeType.PineForestOutskirts, BiomeType.RockyFlatland },
            new[] { BiomeType.Rockyland,           BiomeType.Rockyland,           BiomeType.Rockyland,           BiomeType.RockylandOutskirts,  BiomeType.RockylandOutskirts }
        })
    };

    public static Dictionary<BiomeType, BiomeObjectData> BiomeObjectPool = new() // TODO: this needs a rewrite when custom Random util class is created
    {
        // FLATLANDS
        {
            BiomeType.Flatland,
            new BiomeObjectData(5.0f, new[]
            {
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.SmallTree
            })
        },
        {
            BiomeType.BushFlatland,
            new BiomeObjectData(8.0f, new[]
            {
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.BerryBush
            })
        },
        {
            BiomeType.DenseBushFlatland,
            new BiomeObjectData(25.0f, new[]
            {
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.BerryBush
            })
        },
        {
            BiomeType.RockyFlatland,
            new BiomeObjectData(8.0f, new[]
            {
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.MediumRock,
                EnvironmentObjectId.MediumRock
            })
        },
        
        // FORESTS
        {
            BiomeType.OakForest,
            new BiomeObjectData(25.0f, new[]
            {
                EnvironmentObjectId.OakTree,
                EnvironmentObjectId.OakTree,
                EnvironmentObjectId.OakTree,
                EnvironmentObjectId.OakTree,
                EnvironmentObjectId.AppleTree,
                EnvironmentObjectId.AppleTree,
                EnvironmentObjectId.AppleTree,
                EnvironmentObjectId.BigBush,
                EnvironmentObjectId.SmallBush,
                EnvironmentObjectId.BerryBush
            })
        },
        {
            BiomeType.OakForestOutskirts,
            new BiomeObjectData(15.0f, new[]
            {
                EnvironmentObjectId.OakTree,
                EnvironmentObjectId.OakTree,
                EnvironmentObjectId.OakTree,
                EnvironmentObjectId.AppleTree,
                EnvironmentObjectId.SmallBush
            })
        },
        {
            BiomeType.PineForest,
            new BiomeObjectData(25.0f, new[]
            {
                EnvironmentObjectId.PineTree,
                EnvironmentObjectId.PineTree,
                EnvironmentObjectId.PineTree,
                EnvironmentObjectId.PineTree,
                EnvironmentObjectId.BerryBush,
                EnvironmentObjectId.SmallBush
            })
        },
        {
            BiomeType.PineForestOutskirts,
            new BiomeObjectData(15.0f, new[]
            {
                EnvironmentObjectId.PineTree,
                EnvironmentObjectId.PineTree,
                EnvironmentObjectId.PineTree,
                EnvironmentObjectId.SmallBush,
            })
        },
        {
            BiomeType.DensePineForest,
            new BiomeObjectData(25.0f, new[]
            {
                EnvironmentObjectId.LargePineTree,
                EnvironmentObjectId.LargePineTree,
                EnvironmentObjectId.LargePineTree,
                EnvironmentObjectId.PineTree,
                EnvironmentObjectId.BigBush
            })
        },
        
        // ROCKYLANDS
        {
            BiomeType.Rockyland,
            new BiomeObjectData(25.0f, new[]
            {
                EnvironmentObjectId.MediumRock,
                EnvironmentObjectId.LargeRock,
                EnvironmentObjectId.LargeRock,
                EnvironmentObjectId.LargeRock,
                EnvironmentObjectId.GiantRock
            })
        },
        {
            BiomeType.RockylandOutskirts,
            new BiomeObjectData(8.0f, new[]
            {
                EnvironmentObjectId.MediumRock,
                EnvironmentObjectId.MediumRock,
                EnvironmentObjectId.MediumRock,
                EnvironmentObjectId.LargeRock,
                EnvironmentObjectId.SmallBush
            })
        },
        
        // BEACH
        {
            BiomeType.Beach,
            new BiomeObjectData(0.0f, new EnvironmentObjectId[]{})
        },
        
        // OCEAN
        {
            BiomeType.Ocean,
            new BiomeObjectData(0.0f, new EnvironmentObjectId[]{})
        }
    };
}