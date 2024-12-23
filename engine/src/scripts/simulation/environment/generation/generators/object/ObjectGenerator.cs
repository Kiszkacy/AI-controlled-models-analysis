
using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

public class ObjectGenerator
{
    private readonly int maxAmountOfSpawnTries;
    private readonly int safeSpaceDistance;
    private readonly float objectDensityMultiplier;

    public virtual EnvironmentObjectData[] Generate(EnvironmentGenerationSettings settings, BiomeType[] biomeData, bool[] terrainData)
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
            float targetAmountOfObjects = settings.BiomeTable.BiomeObjectPool[biomeAtCurrentPosition].AmountOfObjectsPer1000x1000Pixels * this.objectDensityMultiplier;
            float chanceAmount = (settings.TerrainChunkSize.X * settings.TerrainChunkSize.Y / (1000 * 1000)) * targetAmountOfObjects;
            LinkedList<EnvironmentObjectData> currentChunkData = new();

            while (chanceAmount > 0.0f)
            {
                if (chanceAmount <= 1.0f && !RandomGenerator.Occurs(chanceAmount))
                {
                    break;
                }

                Vector2? possiblePosition = this.GetValidObjectPosition(currentChunkPosition, currentChunkData, settings.TerrainChunkSize);
                if (possiblePosition.HasValue)
                {
                    Vector2 position = possiblePosition.Value;
                    EnvironmentObjectId objectId = this.PickObjectId(settings.BiomeTable, biomeAtCurrentPosition);
                    currentChunkData.AddLast(new EnvironmentObjectData(objectId, position));
                }
                chanceAmount -= 1.0f;
            }

            data.Extend(currentChunkData);

            currentChunkPosition.X += settings.TerrainChunkSize.X;
            if (currentChunkPosition.X >= settings.Size.X)
            {
                currentChunkPosition.X = 0;
                currentChunkPosition.Y += settings.TerrainChunkSize.Y;
            }
        }

        return data.ToArray();
    }

    private Vector2? GetValidObjectPosition(Vector2 topLeftPosition, LinkedList<EnvironmentObjectData> otherObjectsInThisChunk, Vector2 chunkSize)
    {
        int tryCount = 0;
        bool isValid = false;
        Vector2 position = Vector2.Zero;

        while (tryCount < this.maxAmountOfSpawnTries && !isValid)
        {
            tryCount += 1;
            position = topLeftPosition + new Vector2(
                RandomGenerator.Float(0.0f, chunkSize.X),
                RandomGenerator.Float(0.0f, chunkSize.Y)
            );

            bool isAnyObjectToClose = false;
            foreach (EnvironmentObjectData objectData in otherObjectsInThisChunk)
            {
                if (objectData.Position.DistanceTo(position) <= this.safeSpaceDistance)
                {
                    isAnyObjectToClose = true;
                    break;
                }
            }

            if (!isAnyObjectToClose)
            {
                isValid = true;
            }
        }

        if (isValid)
        {
            return position;
        }
        return null;
    }

    private EnvironmentObjectId PickObjectId(BiomeTable biomeTable, BiomeType biomeType)
    {
        int pickedObjectIndex = RandomGenerator.Index(biomeTable.BiomeObjectPool[biomeType].Weights);
        return biomeTable.BiomeObjectPool[biomeType].Objects[pickedObjectIndex];
    }

    public ObjectGenerator(int maxAmountOfSpawnTries, int safeSpaceDistance, float objectDensityMultiplier)
    {
        this.maxAmountOfSpawnTries = maxAmountOfSpawnTries;
        this.safeSpaceDistance = safeSpaceDistance;
        this.objectDensityMultiplier = objectDensityMultiplier;
    }
}