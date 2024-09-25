
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

    public static Dictionary<BiomeType, BiomeObjectData> BiomeObjectPool = new()
    {
        // FLATLANDS
        {
            BiomeType.Flatland,
            new BiomeObjectData(
                5.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.SmallBush,
                    EnvironmentObjectId.SmallTree
                }, 
                weights: new[]
                {
                    4, 
                    1
                }
            )
        },
        {
            BiomeType.BushFlatland,
            new BiomeObjectData(
                8.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.SmallBush,
                    EnvironmentObjectId.BerryBush
                }, 
                weights: new[]
                {
                    3, 
                    1
                }
            )
        },
        {
            BiomeType.DenseBushFlatland,
            new BiomeObjectData(
                25.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.SmallBush,
                    EnvironmentObjectId.BerryBush
                }, 
                weights: new[]
                {
                    1, 
                    1
                }
            )
        },
        {
            BiomeType.RockyFlatland,
            new BiomeObjectData(
                8.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.SmallBush,
                    EnvironmentObjectId.MediumRock
                }, 
                weights: new[]
                {
                    1, 
                    2
                }
            )
        },
        
        // FORESTS
        {
            BiomeType.OakForest,
            new BiomeObjectData(
                25.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.OakTree,
                    EnvironmentObjectId.AppleTree,
                    EnvironmentObjectId.BigBush,
                    EnvironmentObjectId.SmallBush,
                    EnvironmentObjectId.BerryBush
                }, 
                weights: new[]
                {
                    4, 
                    3,
                    1,
                    1,
                    1
                }
            )
        },
        {
            BiomeType.OakForestOutskirts,
            new BiomeObjectData(
                15.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.OakTree,
                    EnvironmentObjectId.AppleTree,
                    EnvironmentObjectId.SmallBush
                }, 
                weights: new[]
                {
                    3, 
                    1,
                    1
                }
            )
        },
        {
            BiomeType.PineForest,
            new BiomeObjectData(
                25.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.PineTree,
                    EnvironmentObjectId.BerryBush,
                    EnvironmentObjectId.SmallBush
                }, 
                weights: new[]
                {
                    4, 
                    1,
                    1
                }
            )
        },
        {
            BiomeType.PineForestOutskirts,
            new BiomeObjectData(
                15.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.PineTree,
                    EnvironmentObjectId.SmallBush
                }, 
                weights: new[]
                {
                    3, 
                    1
                }
            )
        },
        {
            BiomeType.DensePineForest,
            new BiomeObjectData(
                25.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.LargePineTree,
                    EnvironmentObjectId.PineTree,
                    EnvironmentObjectId.BigBush
                }, 
                weights: new[]
                {
                    3, 
                    1,
                    1
                }
            )
        },
        
        // ROCKYLANDS
        {
            BiomeType.Rockyland,
            new BiomeObjectData(
                25.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.MediumRock,
                    EnvironmentObjectId.LargeRock,
                    EnvironmentObjectId.GiantRock
                }, 
                weights: new[]
                {
                    1, 
                    3,
                    1
                }
            )
        },
        {
            BiomeType.RockylandOutskirts,
            new BiomeObjectData(
                8.0f, 
                objects: new[]
                {
                    EnvironmentObjectId.MediumRock,
                    EnvironmentObjectId.LargeRock,
                    EnvironmentObjectId.SmallBush
                }, 
                weights: new[]
                {
                    3, 
                    1,
                    1
                }
            )
        },
        
        // BEACH
        {
            BiomeType.Beach,
            new BiomeObjectData(0.0f, Array.Empty<EnvironmentObjectId>(), Array.Empty<int>())
        },
        
        // OCEAN
        {
            BiomeType.Ocean,
            new BiomeObjectData(0.0f, Array.Empty<EnvironmentObjectId>(), Array.Empty<int>())
        }
    };
}