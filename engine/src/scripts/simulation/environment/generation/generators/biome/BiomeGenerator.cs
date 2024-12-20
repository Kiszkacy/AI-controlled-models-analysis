
using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

public class BiomeGenerator
{
    private readonly NoiseGenerator noise1;
    private readonly NoiseGenerator noise2;
    private readonly float distanceRandomness;
    private readonly BiomeTableRow[] biomes;

    public BiomeType[] Generate(EnvironmentGenerationSettings settings)
    {
        float maxDistance = Mathf.Min(settings.Size.X / 2.0f, settings.Size.Y / 2.0f) * settings.OceanSizeMultiplier;
        Vector2 currentChunkPosition = Vector2.Zero;
        LinkedList<BiomeType> data = new();

        while (true)
        {
            Vector2 currentChunkCenter = currentChunkPosition + settings.BiomeChunkSize / 2.0f;
            float distanceToClosestTerrainPoint = settings.TerrainPoints.Length == 0
                ? maxDistance
                : settings.TerrainPoints.Min(point => currentChunkCenter.DistanceTo(point * settings.Size));
            float distanceToClosestOceanPoint = settings.OceanPoints.Length == 0
                ? maxDistance
                : settings.OceanPoints.Min(point => currentChunkCenter.DistanceTo(point * settings.Size));

            float normalizedDistanceToClosestTerrainPoint = distanceToClosestTerrainPoint / maxDistance;
            float normalizedDistanceToClosestOceanPoint = distanceToClosestOceanPoint / maxDistance;


            float distanceValue = Mathf.Clamp(
                normalizedDistanceToClosestTerrainPoint + (1.0f-normalizedDistanceToClosestOceanPoint) + RandomGenerator.Float(-this.distanceRandomness, this.distanceRandomness),
                0.0f,
                1.0f
            );

            float noise1 = Mathf.Remap(this.noise1.At(currentChunkCenter/1000.0f), -1, 1, 0, 1);
            float noise2 = Mathf.Remap(this.noise2.At(currentChunkCenter/1000.0f), -1, 1, 0, 1);

            BiomeType biomeType = this.GetBiomeType(distanceValue, noise1, noise2);
            data.AddLast(biomeType);

            currentChunkPosition.X += settings.BiomeChunkSize.X;
            if (currentChunkPosition.X >= settings.Size.X)
            {
                currentChunkPosition.X = 0;
                currentChunkPosition.Y += settings.BiomeChunkSize.Y;
            }
            if (currentChunkPosition.Y >= settings.Size.Y)
            {
                break;
            }
        }

        return data.ToArray();
    }

    private BiomeType GetBiomeType(float distance, float noise1, float noise2)
    {
        if (distance > 1.0f)
        {
            return BiomeType.Ocean;
        }

        BiomeTableRow? possibleRow = null;
        foreach (BiomeTableRow biomeRow in this.biomes)
        {
            if (distance >= biomeRow.DistanceFromCenterStart && distance <= biomeRow.DistanceFromCenterEnd)
            {
                possibleRow = biomeRow;
                break;
            }
        }

        if (!possibleRow.HasValue)
        {
            throw new ArgumentException("Could not select correct biomeTableRow according to given distance.");
        }

        BiomeTableRow row = possibleRow.Value;
        int targetBiomeRowIndex = (int)((noise1 * row.Biomes.Length) / 1.0f);

        BiomeType[] targetBiomeRow = row.Biomes[targetBiomeRowIndex];
        int targetBiomeIndex = (int)((noise2 * targetBiomeRow.Length) / 1.0f);

        return targetBiomeRow[targetBiomeIndex];
    }

    public BiomeGenerator(NoiseGenerator noise1, NoiseGenerator noise2, float distanceRandomness, BiomeTableRow[] biomes)
    {
        this.noise1 = noise1;
        this.noise2 = noise2;
        this.distanceRandomness = distanceRandomness;
        this.biomes = biomes;
    }
}