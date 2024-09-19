
using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

public class BiomeGenerator
{
    private readonly NoiseGenerator noise1;
    private readonly NoiseGenerator noise2;
    private readonly BiomeTableRow[] biomes;

    public BiomeType[] Generate(EnvironmentGenerationSettings settings)
    {
        Vector2 mapCenter = settings.Size / 2.0f;
        float maxDistance = Mathf.Min(settings.Size.Y / 2.0f, settings.Size.X / 2.0f);
        Vector2 currentChunkPosition = Vector2.Zero;
        LinkedList<BiomeType> data = new();

        while (true)
        {
            Vector2 currentChunkCenter = currentChunkPosition + settings.BiomeChunkSize / 2.0f;
            // TODO distance randomness below is hardcoded
            float distanceFromCenter = Mathf.Max((currentChunkCenter.DistanceTo(mapCenter) / maxDistance)+ (new Random().NextSingle()-0.5f)*0.05f, 0.0f);
            float noise1 = Mathf.Remap(this.noise1.At(currentChunkCenter/1000.0f), -1, 1, 0, 1);
            float noise2 = Mathf.Remap(this.noise2.At(currentChunkCenter/1000.0f), -1, 1, 0, 1);

            BiomeType biomeType = GetBiomeType(distanceFromCenter, noise1, noise2);
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

    private BiomeType GetBiomeType(float distance, float noise1, float noise2) // TODO: przejrzyj mnie i posprzataj, kod z chatu
    {
        if (distance > 1.0f)
        {
            return BiomeType.Ocean;
        }

        BiomeTableRow? row_ = null;
        foreach (BiomeTableRow biomeRow in this.biomes)
        {
            if (distance >= biomeRow.DistanceFromCenterStart && distance <= biomeRow.DistanceFromCenterEnd)
            {
                row_ = biomeRow;
                break;
            }
        }

        if (!row_.HasValue)
        {
            throw new ArgumentException("Distance is out of range.");
        }

        BiomeTableRow row = row_.Value;

        int numArrays = row.Biomes.Length;
        int segmentIndex = (int)((noise1 * numArrays) / 1.0f);

        // Ensure the index is within bounds
        segmentIndex = Mathf.Clamp(segmentIndex, 0, numArrays - 1);

        // Get the selected biome array
        BiomeType[] biomeArray = row.Biomes[segmentIndex];

        // Determine the biome type based on noise2
        int numBiomes = biomeArray.Length;
        int biomeIndex = (int)((noise2 * numBiomes) / 1.0f);

        // Ensure the index is within bounds
        biomeIndex = Mathf.Clamp(biomeIndex, 0, numBiomes - 1);

        return biomeArray[biomeIndex];
    }

    public BiomeGenerator(NoiseGenerator noise1, NoiseGenerator noise2, BiomeTableRow[] biomes)
    {
        this.noise1 = noise1;
        this.noise2 = noise2;
        this.biomes = biomes;
    }
}