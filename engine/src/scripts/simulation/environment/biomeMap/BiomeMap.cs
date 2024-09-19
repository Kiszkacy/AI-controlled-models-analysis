using System.Linq;

using Godot;

public partial class BiomeMap : TileMap
{
    [Export]
    public Environment Environment { get; set; }

    public override void _Ready()
    {
        this.Initialize();
    }

    public void Initialize()
    {
        EnvironmentGenerationSettings settings = this.Environment.TemplateData.GenerationSettings;

        this.GlobalScale = new Vector2(1, 1) * settings.TerrainChunkSize;
                           
        BiomeType[] biomeData = this.Environment.TemplateData.BiomeData;
        bool[] terrainData = this.Environment.TemplateData.TerrainData;
        Vector2 terrainSize = settings.TerrainChunkSize;
        int terrainChunksInARow = (int)(settings.Size.X / terrainSize.X) + (settings.Size.X % terrainSize.X != 0 ? 1 : 0);
        int terrainChunksInAColumn = this.Environment.TemplateData.TerrainData.Length / terrainChunksInARow;

        for (int y = 0; y < terrainChunksInAColumn; y++)
        {
            for (int x = 0; x < terrainChunksInARow; x++)
            {
                BiomeType biomeType = BiomeType.Ocean;
                if (terrainData[x + y * terrainChunksInARow])
                {
                    biomeType = EnvironmentGenerationUtil.GetBiomeAt(settings.TerrainChunkSize*new Vector2(x+0.5f, y+0.5f), settings.Size, settings.BiomeChunkSize, biomeData);
                }
            
                this.SetCell(0, new Vector2I(x, y), 1, new Vector2I((int)biomeType, 0));
            }
        }
    }
}
