
using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

public class TerrainGenerator
{
    public int PassCount { get; }
    public int MinimalRequiredOceanNeighborCountToCorrode { get; }
    public float CorrosionChance { get; }
    
    public bool[] Generate(EnvironmentGenerationSettings settings, BiomeType[] biomeData)
    {
        bool[] isIslandAccordingToBiomes = GetIslandChunksAccordingToBiomeData(settings, biomeData);
        
        bool[] isIslandAfterCorrosion = CorrodeIslandChunks(settings, isIslandAccordingToBiomes);

        return isIslandAfterCorrosion;
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

    private bool[] CorrodeIslandChunks(EnvironmentGenerationSettings settings, bool[] isIsland)
    {
        LinkedList<bool> result = new();
        int chunksInARow = EnvironmentGenerationUtil.ChunksInARow(settings.Size, settings.TerrainChunkSize);

        // TODO add pass count
        for (int index = 0; index < isIsland.Length; index++)
        {
            if (!isIsland[index])
            {
                result.AddLast(false);
                continue;
            }
            
            int oceanNeighborCount = GetOceanNeighborCount(index, chunksInARow, isIsland);
            if (oceanNeighborCount < this.MinimalRequiredOceanNeighborCountToCorrode)
            {
                result.AddLast(true);
                continue;
            }
        
            result.AddLast(!(new Random().NextDouble() <= this.CorrosionChance));
        }
        
        return result.ToArray();
    }

    private static int GetOceanNeighborCount(int index, int chunksInArow, bool[] isIsland)
    {
        int count = 0;

        if (index-1-chunksInArow >= 0 && !isIsland[index-1-chunksInArow])
        {
            count += 1;
        }
        if (index-chunksInArow >= 0 && !isIsland[index-chunksInArow])
        {
            count += 1;
        }
        if (index-chunksInArow+1 >= 0 && !isIsland[index-chunksInArow+1])
        {
            count += 1;
        }
        if (index-1 >= 0 && !isIsland[index-1])
        {
            count += 1;
        }
        if (index+1 < isIsland.Length && !isIsland[index+1])
        {
            count += 1;
        }
        if (index-1+chunksInArow < isIsland.Length && !isIsland[index-1+chunksInArow])
        {
            count += 1;
        }
        if (index+chunksInArow < isIsland.Length && !isIsland[index+chunksInArow])
        {
            count += 1;
        }
        if (index+1+chunksInArow < isIsland.Length && !isIsland[index+1+chunksInArow])
        {
            count += 1;
        }
        
        return count;
    }

    public TerrainGenerator(int passCount, int minimalRequiredOceanNeighborCountToCorrode, float corrosionChance)
    {
        this.PassCount = passCount;
        this.MinimalRequiredOceanNeighborCountToCorrode = minimalRequiredOceanNeighborCountToCorrode;
        this.CorrosionChance = corrosionChance;
    }
}