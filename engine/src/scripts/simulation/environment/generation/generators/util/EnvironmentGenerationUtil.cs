
using Godot;

public static class EnvironmentGenerationUtil
{
    // TODO refactor this and move to global util function, its used in many places not only during generation
    // TODO or remove it completely and handle biomeData in different way, with more data for ease of use
    // TODO biomeData could store its position etc
    public static BiomeType GetBiomeAt(Vector2 position, Vector2 size, Vector2 biomeChunkSize, BiomeType[] biomeData)
    {
        int biomeChunksInARow = ChunksInARow(size.X, biomeChunkSize.X);

        int targetBiomeChunkColumnIndex = (int)(position.X / biomeChunkSize.X);
        int targetBiomeChunkRowIndex = (int)(position.Y / biomeChunkSize.Y);

        return biomeData[targetBiomeChunkRowIndex * biomeChunksInARow + targetBiomeChunkColumnIndex];
    }

    public static int ChunksInARow(float environmentWidth, float chunkWidth)
    {
        return (int)(environmentWidth / chunkWidth) + (environmentWidth % chunkWidth != 0 ? 1 : 0);
    }
}