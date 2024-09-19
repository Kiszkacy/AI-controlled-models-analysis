
using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

public class TerrainGenerator
{
    public bool[] Generate(EnvironmentGenerationSettings settings, BiomeType[] biomeData)
    {
        bool[] isIslandAccordingToBiomes = GetIslandChunksAccordingToBiomeData(settings, biomeData);

        // TODO: do some steps with noise

        return isIslandAccordingToBiomes;
    }

    private static bool[] GetIslandChunksAccordingToBiomeData(EnvironmentGenerationSettings settings, BiomeType[] biomeData)
    {
        Vector2 currentChunkPosition = Vector2.Zero;
        LinkedList<bool> isIsland = new();
        
        while (true)
        {
            BiomeType biomeAtCurrentPosition = EnvironmentGenerationUtil.GetBiomeAt(currentChunkPosition, settings.Size, settings.BiomeChunkSize, biomeData);
            isIsland.AddLast(biomeAtCurrentPosition != BiomeType.Ocean);
            
            currentChunkPosition.X += settings.TerrainChunkSize.X;
            if (currentChunkPosition.X >= settings.Size.X)
            {
                currentChunkPosition.X = 0;
                currentChunkPosition.Y += settings.TerrainChunkSize.Y;
            }
            if (currentChunkPosition.Y >= settings.Size.Y)
            {
                break;
            }
        }

        return isIsland.ToArray();
    }
    
}