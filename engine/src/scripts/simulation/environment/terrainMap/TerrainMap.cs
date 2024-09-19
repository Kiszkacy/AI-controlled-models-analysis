
using Godot;

public partial class TerrainMap : TileMap
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
                           
        bool[] terrainData = this.Environment.TemplateData.TerrainData;
        Vector2 terrainSize = settings.TerrainChunkSize;
        int terrainChunksInARow = (int)(settings.Size.X / terrainSize.X) + (settings.Size.X % terrainSize.X != 0 ? 1 : 0);
        int terrainChunksInAColumn = terrainData.Length / terrainChunksInARow;

        for (int y = 0; y < terrainChunksInAColumn; y++)
        {
            for (int x = 0; x < terrainChunksInARow; x++)
            {
                bool isTerrain = terrainData[x + y*terrainChunksInARow];

                this.SetCell(0, new Vector2I(x, y), 0, new Vector2I(isTerrain ? 1 : 0,0));
            }
        }
    }
}
