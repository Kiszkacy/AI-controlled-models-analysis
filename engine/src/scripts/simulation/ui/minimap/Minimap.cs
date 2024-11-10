using Godot;

[System.Obsolete]
public partial class Minimap : TileMap
{
    // sizeOfTileInPixels = 8 (change inside godot editor)
    // sizeOfTileInPixels * sizeInTiles = 256 !!
    public const int sizeInTiles = 32;

    public override void _Ready()
    {
        this.ReloadMinimap();
    }

    private void ReloadMinimap()
    {
        Vector2 environmentSize = EnvironmentManager.Instance.Environment.Size;
        Vector2 tileSize = environmentSize / sizeInTiles;

        for (int y = 0; y < sizeInTiles; y++)
        {
            Vector2 currentPosition = new(tileSize.X / 2.0f, tileSize.Y / 2.0f + tileSize.Y * y);
            for (int x = 0; x < sizeInTiles; x++)
            {
                BiomeType biomeType = EnvironmentManager.Instance.GetBiomeAt(currentPosition);
                this.SetCell(0, new Vector2I(x, y), 0, new Vector2I((int)biomeType, 0));
                currentPosition.X += tileSize.X;
            }
        }
    }
}