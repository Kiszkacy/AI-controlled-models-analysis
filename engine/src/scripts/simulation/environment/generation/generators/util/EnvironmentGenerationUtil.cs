
using Godot;

public static class EnvironmentGenerationUtil
{
    public static BiomeType GetBiomeAt(Vector2 position, Vector2 size, Vector2 biomeChunkSize, BiomeType[] biomeData)
    {
        int biomeChunksInARow = ChunksInARow(size, biomeChunkSize);

        int targetBiomeChunkColumnIndex = (int)(position.X / biomeChunkSize.X);
        int targetBiomeChunkRowIndex = (int)(position.Y / biomeChunkSize.Y);

        return biomeData[targetBiomeChunkRowIndex * biomeChunksInARow + targetBiomeChunkColumnIndex];
    }

    public static int ChunksInARow(Vector2 size, Vector2 chunkSize)
    {
        return (int)(size.X / chunkSize.X) + (size.X % chunkSize.X != 0 ? 1 : 0);
    }
}