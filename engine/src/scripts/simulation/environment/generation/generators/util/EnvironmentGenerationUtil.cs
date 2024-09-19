
using Godot;

public static class EnvironmentGenerationUtil
{
    public static BiomeType GetBiomeAt(Vector2 position, Vector2 size, Vector2 biomeChunkSize, BiomeType[] biomeData)
    {
        int biomeChunksInARow = (int)(size.X / biomeChunkSize.X) + (size.X % biomeChunkSize.X != 0 ? 1 : 0);
        
        int targetBiomeChunkColumnIndex = (int)(position.X / biomeChunkSize.X);
        int targetBiomeChunkRowIndex = (int)(position.Y / biomeChunkSize.Y);
        
        return biomeData[targetBiomeChunkRowIndex * biomeChunksInARow + targetBiomeChunkColumnIndex];
    }
    
    public static bool IsTerrainAt(Vector2 position, Vector2 size, Vector2 terrainChunkSize, bool[] terrainData)
    {
        int terrainChunksInARow = (int)(size.X / terrainChunkSize.X) + (size.X % terrainChunkSize.X != 0 ? 1 : 0);
        
        int targetTerrainChunkColumnIndex = (int)(position.X / terrainChunkSize.X);
        int targetTerrainChunkRowIndex = (int)(position.Y / terrainChunkSize.Y);

        return terrainData[targetTerrainChunkRowIndex * terrainChunksInARow + targetTerrainChunkColumnIndex];
    }
}