using Godot;

[System.Obsolete]
public partial class Minimap : TileMap
{
    [Export]
    public int SizeInTiles { get; set; } = 32;

    [Export]
    public Environment Environment { get; set; } = null;

    public bool DrawLandOnlyMode { get; set; } = false;

    public override void _Ready()
    {
        this.Refresh();
    }

    public void Refresh()
    {
        this.Clear();

        Environment environment = this.Environment ?? EnvironmentManager.Instance.Environment;
        Vector2 environmentSize = environment.Size;
        Vector2 tileSize = environmentSize / this.SizeInTiles;

        for (int y = 0; y < this.SizeInTiles; y++)
        {
            Vector2 currentPosition = new(tileSize.X / 2.0f, tileSize.Y / 2.0f + tileSize.Y * y);
            for (int x = 0; x < this.SizeInTiles; x++)
            {
                if (this.DrawLandOnlyMode)
                {
                    bool isLandCell = environment.GetBiomeAt(currentPosition) != BiomeType.Ocean;
                    this.SetCell(0, new Vector2I(x, y), 0, new Vector2I((int)(isLandCell ? (BiomeType.Rockyland) : BiomeType.Ocean), 0));
                }
                else
                {
                    BiomeType biomeType = environment.GetBiomeAt(currentPosition);
                    this.SetCell(0, new Vector2I(x, y), 0, new Vector2I((int)biomeType, 0));
                }
                currentPosition.X += tileSize.X;
            }
        }
    }
}