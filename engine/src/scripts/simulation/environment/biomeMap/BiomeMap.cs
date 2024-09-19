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

        this.GlobalScale = new Vector2(1, 1) * settings.BiomeChunkSize;
                           
        BiomeType[] biomeData = this.Environment.TemplateData.BiomeData;
        Vector2 biomeSize = settings.BiomeChunkSize;
        int biomeChunksInARow = (int)(settings.Size.X / biomeSize.X) + (settings.Size.X % biomeSize.X != 0 ? 1 : 0);
        int biomeChunksInAColumn = biomeData.Length / biomeChunksInARow;

        for (int y = 0; y < biomeChunksInAColumn; y++)
        {
            for (int x = 0; x < biomeChunksInARow; x++)
            {
                BiomeType biomeType = biomeData[x + y*biomeChunksInARow];

                this.SetCell(0, new Vector2I(x, y), 1, new Vector2I((int)biomeType, 0));
            }
        }
    }
}
