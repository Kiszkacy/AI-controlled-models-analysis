
using Godot;
using Godot.Collections;

public struct EnvironmentGenerationSettings
{
    public Vector2 Size { get; set; }
    public Vector2 BiomeChunkSize { get; set;  }
    public Vector2 TerrainChunkSize { get; set;  }

    public EnvironmentGenerationSettings(Vector2 size, Vector2 biomeChunkSize, Vector2 terrainChunkSize)
    {
        this.Size = size;
        this.BiomeChunkSize = biomeChunkSize;
        this.TerrainChunkSize = terrainChunkSize;
    }
    
    public Dictionary ToDictionary()
    {
        var dict = new Dictionary
        {
            { "Size", this.Size },
            { "BiomeChunkSize", this.BiomeChunkSize },
            { "TerrainChunkSize", this.TerrainChunkSize }
        };

        return dict;
    }

    public static EnvironmentGenerationSettings FromDictionary(Dictionary data)
    {
        Vector2 size = (Vector2)data["Size"];
        Vector2 biomeChunkSize = (Vector2)data["BiomeChunkSize"];
        Vector2 terrainChunkSize = (Vector2)data["TerrainChunkSize"];
        
        return new EnvironmentGenerationSettings(size, biomeChunkSize, terrainChunkSize);
    }
}