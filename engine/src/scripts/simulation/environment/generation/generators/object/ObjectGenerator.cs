
using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

public class ObjectGenerator
{
    private readonly int maxAmountOfSpawnTriesIfFailed;

    public EnvironmentObjectData[] Generate(EnvironmentGenerationSettings settings, BiomeType[] biomeData, bool[] terrainData)
    {
        LinkedList<EnvironmentObjectData> data = new();
        Vector2 currentChunkPosition = Vector2.Zero;

        foreach (bool isTerrain in terrainData)
        {
            if (!isTerrain)
            {
                currentChunkPosition.X += settings.TerrainChunkSize.X;
                if (currentChunkPosition.X >= settings.Size.X)
                {
                    currentChunkPosition.X = 0;
                    currentChunkPosition.Y += settings.TerrainChunkSize.Y;
                }
                continue;
            }

            Vector2 currentChunkCenter = currentChunkPosition + settings.TerrainChunkSize / 2.0f;
            BiomeType biomeAtCurrentPosition = EnvironmentGenerationUtil.GetBiomeAt(currentChunkCenter, settings.Size, settings.BiomeChunkSize, biomeData);
            float targetAmountOfObjects = BiomeTable.BiomeObjectPool[biomeAtCurrentPosition].AmountOfObjectsPer1000x1000Pixels;
            float chanceAmount = (settings.TerrainChunkSize.X * settings.TerrainChunkSize.Y / (1000 * 1000)) * targetAmountOfObjects;

            while (chanceAmount > 0.0f)
            {
                if (chanceAmount <= 1.0f && !(new Random().NextDouble() <= chanceAmount))
                {
                    break;
                }

                Vector2? possiblePosition = this.GetValidObjectPosition(currentChunkPosition, settings.TerrainChunkSize);
                if (possiblePosition.HasValue)
                {
                    Vector2 position = possiblePosition.Value;
                    EnvironmentObjectId objectId = this.PickObjectId(biomeAtCurrentPosition);
                    data.AddLast(new EnvironmentObjectData(objectId, position));
                }
                chanceAmount -= 1.0f;
            }

            currentChunkPosition.X += settings.TerrainChunkSize.X;
            if (currentChunkPosition.X >= settings.Size.X)
            {
                currentChunkPosition.X = 0;
                currentChunkPosition.Y += settings.TerrainChunkSize.Y;
            }
        }

        return data.ToArray();
    }

    private Vector2? GetValidObjectPosition(Vector2 topLeftPosition, Vector2 chunkSize)
    {
        // TODO: here add missing safe space checks and spawnTries using maxAmountOfSpawnTriesIfFailed
        // int tryCount = 0;
        // while (tryCount < this.maxAmountOfSpawnTriesIfFailed) 
        // {
        //         
        // }
        Vector2 position = new(
            topLeftPosition.X + (float)(new Random().NextDouble()) * chunkSize.X,
            topLeftPosition.Y + (float)(new Random().NextDouble()) * chunkSize.Y
        );

        return position;
    }
    
    private EnvironmentObjectId PickObjectId(BiomeType biomeType)
    {
        int pickedObjectIndex = RandomGenerator.Index(BiomeTable.BiomeObjectPool[biomeType].Weights);
        return BiomeTable.BiomeObjectPool[biomeType].Objects[pickedObjectIndex];
    }
}