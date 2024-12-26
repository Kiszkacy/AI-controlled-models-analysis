using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Godot;
using Godot.Collections;

public partial class EnvironmentConfiguration : Control
{
    [Export]
    [System.Obsolete]
    private Minimap Minimap;
    [Export]
    private PointDrawer LandPointsDrawer;
    [Export]
    private PointDrawer OceanPointsDrawer;

    [Export]
    private InputHandler SimulationName;
    [Export]
    private RichTextLabel SimulationNameSaveLocation;
    [Export]
    private InputHandler SimulationSeed;
    [Export]
    private InputHandler SelectedMapSizeButton;
    [Export]
    private MenuButton MapSizeButton;
    [Export]
    private CheckButton ShowBiomesButton;
    [Export]
    private CheckButton ShowLandPointsButton;
    [Export]
    private CheckButton ShowOceanPointsButton;

    [Export]
    private Control BigMinimapParent;
    [Export]
    [System.Obsolete]
    private Minimap BigMinimap;
    [Export]
    private Control BigMinimapArea;
    [Export]
    private PointDrawer BigLandPointsDrawer;
    [Export]
    private PointDrawer BigOceanPointsDrawer;
    [Export]
    private ButtonHandler BigMinimapButton;
    [Export]
    private ButtonHandler CloseBigMinimapButton;

    [Export]
    private Slider OceanSizeSlider;
    [Export]
    private InputHandler OceanSize;
    [Export]
    private InputHandler SelectedBiomeChunkSizeButton;
    [Export]
    private MenuButton BiomeChunkSizeButton;
    [Export]
    private InputHandler SelectedBiomeTableButton;
    [Export]
    private MenuButton BiomeTableButton;

    private bool inBigMapMode = false;
    private bool InBigMapMode
    {
        get => this.inBigMapMode;
        set
        {
            this.BigMinimapParent.Visible = value;
            this.inBigMapMode = value;
        }
    }
    private bool isOceanSizeDragging = false;

    private const int SmallSimulationSize = 10000;
    private const int MediumSimulationSize = 20000;
    private const int LargeSimulationSize = 40000;
    private const int MinimapSize = 448;
    private const int SmallSimulationMinimapTileCount = 56;
    private const int MediumSimulationMinimapTileCount = 64;
    private const int LargeSimulationMinimapTileCount = 112;
    private const int BigMinimapSize = 768;
    private const int SmallSimulationBigMinimapTileCount = 96;
    private const int MediumSimulationBigMinimapTileCount = 128;
    private const int LargeSimulationBigMinimapTileCount = 192;
    private const float MinOceanSize = 0.5f;
    private const float MaxOceanSize = 5.0f;
    private const float ExtraSmallBiomeChunkSize = 20.0f;
    private const float SmallBiomeChunkSize = 25.0f;
    private const float MediumBiomeChunkSize = 50.0f;
    private const float LargeBiomeChunkSize = 100.0f;

    private const float SmallTerrainChunkSize = 20.0f;
    private const float MediumTerrainChunkSize = 25.0f;
    private const float LargeTerrainChunkSize = 50.0f;

    private Environment environment;
    private EnvironmentGenerator environmentGenerator;
    private bool showLandPoints = false;
    private bool showOceanPoints = false;

    private string simulationName = null;
    private int simulationSeed = 42;
    private Vector2 simulationSize = new(SmallSimulationSize, SmallSimulationSize);

    private readonly LinkedList<Vector2> terrainPoints = new(new[] { new Vector2(0.5f, 0.5f) });
    private readonly LinkedList<Vector2> oceanPoints = new();

    private float oceanSizeMultiplier = 1.0f;
    private Vector2 biomeChunkSize = new(SmallBiomeChunkSize, SmallBiomeChunkSize);
    private BiomeTable biomeTable = BiomeTable.Default;


    private Vector2 terrainChunkSize = new(MediumTerrainChunkSize, MediumTerrainChunkSize);
    private float corrosionChance = 0.5f;
    private float corrosionNeighborCount = 3; // CorrosionNeighborCountInput

    public override void _Ready()
    {
        this.ConnectEvents();

        this.SimulationNameSaveLocation.Text = $"{ProjectSettings.GlobalizePath(Config.Instance.Save.SavePath)}default.gsave";
        this.SimulationSeed.SetTextTo(this.simulationSeed.ToString());
        RandomGenerator.SetSeed(this.simulationSeed);

        this.RebuildGeneratorAndEnvironment();
    }

    private void ConnectEvents()
    {
        this.SimulationName.TextSubmitted += this.OnSimulationNameChange;
        this.SimulationSeed.TextSubmitted += this.OnSimulationSeedChange;
        this.MapSizeButton.GetPopup().IndexPressed += this.OnMapSizeClick;
        this.ShowBiomesButton.Pressed += this.OnShowBiomesChange;
        this.ShowLandPointsButton.Pressed += this.OnShowLandPointsChange;
        this.ShowOceanPointsButton.Pressed += this.OnShowOceanPointsChange;
        this.BigMinimapButton.Pressed += this.OnBigMinimapClick;
        this.CloseBigMinimapButton.Pressed += this.OnCloseBigMinimapClick;

        this.OceanSize.TextSubmitted += this.OnOceanSizeChange;
        this.OceanSizeSlider.DragEnded += this.OnOceanSizeDragChange;
        this.OceanSizeSlider.DragStarted += this.OnOceanSizeDragStart;
        this.BiomeChunkSizeButton.GetPopup().IndexPressed += this.OnBiomeChunkSizeClick;
        this.BiomeTableButton.GetPopup().IndexPressed += this.OnBiomeTablePresetClick;
    }

    private void OnSimulationNameChange(object _, BaseEventArgs<string> args)
    {
        this.SimulationNameSaveLocation.Text = $"{ProjectSettings.GlobalizePath(Config.Instance.Save.SavePath)}{args.Value ?? "default"}.gsave";
    }

    private void OnSimulationSeedChange(object _, BaseEventArgs<string> args)
    {
        int seed = int.Parse(args.Value);
        this.simulationSeed = seed;
        this.RebuildGeneratorAndEnvironment();
    }

    private void OnMapSizeClick(long selectedItemId)
    {
        switch (selectedItemId)
        {
            case 0:
                this.simulationSize = new Vector2(SmallSimulationSize, SmallSimulationSize);
                this.SelectedMapSizeButton.SetTextTo("Small");
                this.Minimap.SizeInTiles = SmallSimulationMinimapTileCount;
                this.BigMinimap.SizeInTiles = SmallSimulationBigMinimapTileCount;
                this.Minimap.Scale = new Vector2(MinimapSize/SmallSimulationMinimapTileCount, MinimapSize/SmallSimulationMinimapTileCount);
                this.BigMinimap.Scale = new Vector2(BigMinimapSize/SmallSimulationBigMinimapTileCount, BigMinimapSize/SmallSimulationBigMinimapTileCount);
                break;
            case 1:
                this.simulationSize = new Vector2(MediumSimulationSize, MediumSimulationSize);
                this.SelectedMapSizeButton.SetTextTo("Medium");
                this.Minimap.SizeInTiles = MediumSimulationMinimapTileCount;
                this.BigMinimap.SizeInTiles = MediumSimulationBigMinimapTileCount;
                this.Minimap.Scale = new Vector2(MinimapSize/MediumSimulationMinimapTileCount, MinimapSize/MediumSimulationMinimapTileCount);
                this.BigMinimap.Scale = new Vector2(BigMinimapSize/MediumSimulationBigMinimapTileCount, BigMinimapSize/MediumSimulationBigMinimapTileCount);
                break;
            case 2:
                this.simulationSize = new Vector2(LargeSimulationSize, LargeSimulationSize);
                this.SelectedMapSizeButton.SetTextTo("Large");
                this.Minimap.SizeInTiles = LargeSimulationMinimapTileCount;
                this.BigMinimap.SizeInTiles = LargeSimulationBigMinimapTileCount;
                this.Minimap.Scale = new Vector2(MinimapSize/LargeSimulationMinimapTileCount, MinimapSize/LargeSimulationMinimapTileCount);
                this.BigMinimap.Scale = new Vector2(BigMinimapSize/LargeSimulationBigMinimapTileCount, BigMinimapSize/LargeSimulationBigMinimapTileCount);
                break;
        }

        for (int index = 0; index < this.MapSizeButton.GetPopup().ItemCount; index++)
        {
            this.MapSizeButton.GetPopup().SetItemChecked(index, false);
        }
        this.MapSizeButton.GetPopup().SetItemChecked((int)selectedItemId, true);
        this.RebuildGeneratorAndEnvironment();
    }

    private void OnShowBiomesChange()
    {
        this.Minimap.DrawLandOnlyMode = !this.ShowBiomesButton.IsPressed();
        this.Minimap.Refresh();
    }

    private void OnShowLandPointsChange()
    {
        this.showLandPoints = !this.showLandPoints;
        this.LandPointsDrawer.Visible = this.showLandPoints;
    }

    private void OnShowOceanPointsChange()
    {
        this.showOceanPoints = !this.showOceanPoints;
        this.OceanPointsDrawer.Visible = this.showOceanPoints;
    }

    private void OnBigMinimapClick()
    {
        this.InBigMapMode = true;
        this.UpdateMinimap();
    }

    private void OnCloseBigMinimapClick()
    {
        this.InBigMapMode = false;
    }

    private void OnOceanSizeChange(object _, BaseEventArgs<string> args)
    {
        this.isOceanSizeDragging = false;
        float oceanSize = float.Parse(args.Value);
        this.oceanSizeMultiplier = Mathf.Clamp(oceanSize, MinOceanSize, MaxOceanSize);
        this.OceanSize.SetTextTo(oceanSize.ToString());
        this.OceanSizeSlider.SetValueNoSignal(Mathf.Remap(oceanSize, MinOceanSize, MaxOceanSize, 50, 500));

        this.RebuildGeneratorAndEnvironment();
    }

    private void OnOceanSizeDragChange(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }

        this.oceanSizeMultiplier = (float)Mathf.Remap(this.OceanSizeSlider.Value, 50, 500, MinOceanSize, MaxOceanSize);
        this.OceanSize.SetTextTo(this.oceanSizeMultiplier.ToString());

        this.RebuildGeneratorAndEnvironment();
    }

    private void OnOceanSizeDragStart()
    {
        this.isOceanSizeDragging = true;
    }

    private void OnBiomeChunkSizeClick(long selectedItemId)
    {
        switch (selectedItemId)
        {
            case 0:
                this.biomeChunkSize = new Vector2(ExtraSmallBiomeChunkSize, ExtraSmallBiomeChunkSize);
                this.SelectedBiomeChunkSizeButton.SetTextTo($"{ExtraSmallBiomeChunkSize}x{ExtraSmallBiomeChunkSize} pixels");
                break;
            case 1:
                this.biomeChunkSize = new Vector2(SmallBiomeChunkSize, SmallBiomeChunkSize);
                this.SelectedBiomeChunkSizeButton.SetTextTo($"{SmallBiomeChunkSize}x{SmallBiomeChunkSize} pixels");
                break;
            case 2:
                this.biomeChunkSize = new Vector2(MediumBiomeChunkSize, MediumBiomeChunkSize);
                this.SelectedBiomeChunkSizeButton.SetTextTo($"{MediumBiomeChunkSize}x{MediumBiomeChunkSize} pixels");
                break;
            case 3:
                this.biomeChunkSize = new Vector2(LargeBiomeChunkSize, LargeBiomeChunkSize);
                this.SelectedBiomeChunkSizeButton.SetTextTo($"{LargeBiomeChunkSize}x{LargeBiomeChunkSize} pixels");
                break;
        }

        for (int index = 0; index < this.BiomeChunkSizeButton.GetPopup().ItemCount; index++)
        {
            this.BiomeChunkSizeButton.GetPopup().SetItemChecked(index, false);
        }
        this.BiomeChunkSizeButton.GetPopup().SetItemChecked((int)selectedItemId, true);
        this.RebuildGeneratorAndEnvironment();
    }

    private void OnBiomeTablePresetClick(long selectedItemId)
    {
        switch (selectedItemId)
        {
            case 0:
                this.biomeTable = BiomeTable.Default;
                this.SelectedBiomeTableButton.SetTextTo("Default");
                break;
            case 1:
                this.biomeTable = BiomeTable.RandomInland;
                this.SelectedBiomeTableButton.SetTextTo("Random inland");
                break;
            case 2:
                this.biomeTable = BiomeTable.FoodRings;
                this.SelectedBiomeTableButton.SetTextTo("Food circles");
                break;
        }

        for (int index = 0; index < this.BiomeTableButton.GetPopup().ItemCount; index++)
        {
            this.BiomeTableButton.GetPopup().SetItemChecked(index, false);
        }
        this.BiomeTableButton.GetPopup().SetItemChecked((int)selectedItemId, true);
        this.RebuildGeneratorAndEnvironment();
    }

    private void UpdateMinimap()
    {
        this.RedrawMinimapBiomes();
        this.RedrawMinimapPoints();
    }

    private void RedrawMinimapPoints()
    {
        this.LandPointsDrawer.ClearPoints();
        foreach (Vector2 point in this.environmentGenerator.TerrainPoints)
        {
            this.LandPointsDrawer.AddPoint(point * this.LandPointsDrawer.Size);
        }
        this.LandPointsDrawer.QueueRedraw();

        this.OceanPointsDrawer.ClearPoints();
        foreach (Vector2 point in this.environmentGenerator.OceanPoints)
        {
            this.OceanPointsDrawer.AddPoint(point * this.OceanPointsDrawer.Size);
        }
        this.OceanPointsDrawer.QueueRedraw();

        if (this.InBigMapMode)
        {
            this.BigLandPointsDrawer.ClearPoints();
            foreach (Vector2 point in this.environmentGenerator.TerrainPoints)
            {
                this.BigLandPointsDrawer.AddPoint(point * this.BigLandPointsDrawer.Size);
            }
            this.BigLandPointsDrawer.QueueRedraw();

            this.BigOceanPointsDrawer.ClearPoints();
            foreach (Vector2 point in this.environmentGenerator.OceanPoints)
            {
                this.BigOceanPointsDrawer.AddPoint(point * this.BigOceanPointsDrawer.Size);
            }
            this.BigOceanPointsDrawer.QueueRedraw();
        }
    }

    private void RebuildGenerator()
    {
        RandomGenerator.SetSeed(this.simulationSeed);
        this.environmentGenerator = EnvironmentGeneratorBuilder.Start
            .SetAllToDefault()
            .SetSize(this.simulationSize)
            .SetObjectGenerator(ObjectGeneratorBuilder.Start
                .SetAllToDefault()
                .SetGeneratorType(ObjectGeneratorType.Empty)
                .End())
            .SetBiomeChunkSize(this.biomeChunkSize)
            .SetBiomeTable(this.biomeTable)
            .SetTerrainPoints(this.terrainPoints.ToArray())
            .SetOceanPoints(this.oceanPoints.ToArray())
            .SetOceanSizeMultiplier(this.oceanSizeMultiplier)
            .End();

        this.RedrawMinimapPoints();
    }

    private void RebuildEnvironment()
    {
        this.environment = this.environmentGenerator.Generate().Instantiate();
        this.RedrawMinimapBiomes();
    }

    private void RedrawMinimapBiomes()
    {
        this.Minimap.Environment = environment;
        this.Minimap.Refresh();
        if (this.InBigMapMode)
        {
            this.BigMinimap.Environment = environment;
            this.BigMinimap.Refresh();
        }
    }

    private void RebuildGeneratorAndEnvironment()
    {
        this.RebuildGenerator();
        this.RebuildEnvironment();
    }

    public override void _Input(InputEvent @event)
    {
        if (!this.inBigMapMode)
        {
            return;
        }

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            bool terrainPointRelatedClick = mouseEvent.ButtonIndex == MouseButton.Left;
            Vector2 localPosition = this.BigMinimapArea.GetLocalMousePosition();
            Vector2 normalizedPosition = new(
                localPosition.X / this.BigMinimapArea.Size.X,
                localPosition.Y / this.BigMinimapArea.Size.Y
            );

            bool validOnMapClick = normalizedPosition.X is >= 0.0f and <= 1.0f && normalizedPosition.Y is >= 0.0f and <= 1.0f;
            if (!validOnMapClick)
            {
                return;
            }

            Vector2? foundPoint = null;
            LinkedList<Vector2> points = terrainPointRelatedClick ? this.terrainPoints : this.oceanPoints;
            foreach (Vector2 point in points)
            {
                if (IsPointNear(normalizedPosition, point))
                {
                    foundPoint = point;
                    break;
                }
            }

            if (foundPoint.HasValue && terrainPointRelatedClick)
            {
                this.terrainPoints.Remove(foundPoint.Value);
            }
            else if (foundPoint.HasValue && !terrainPointRelatedClick)
            {
                this.oceanPoints.Remove(foundPoint.Value);
            }
            else if (!foundPoint.HasValue && terrainPointRelatedClick)
            {
                this.terrainPoints.AddLast(normalizedPosition);
            }
            else if (!foundPoint.HasValue && !terrainPointRelatedClick)
            {
                this.oceanPoints.AddLast(normalizedPosition);
            }
            this.RebuildGeneratorAndEnvironment();
        }
    }

    public override void _Process(double delta)
    {
        if (this.isOceanSizeDragging)
        {
            float mappedValue = (float)Mathf.Remap(this.OceanSizeSlider.Value, 50, 500, MinOceanSize, MaxOceanSize);
            this.OceanSize.SetTextTo(mappedValue.ToString());
        }
    }

    private bool IsPointNear(Vector2 clickPoint, Vector2 targetPoint, float threshold = 0.01f)
    {
        return clickPoint.DistanceTo(targetPoint) <= threshold;
    }

    public EnvironmentGenerator GetEnvironmentGenerator()
    {
        RandomGenerator.SetSeed(this.simulationSeed);
        return EnvironmentGeneratorBuilder.Start
            .SetAllToDefault()
            .SetSize(this.simulationSize)
            .SetBiomeChunkSize(this.biomeChunkSize)
            .SetBiomeTable(this.biomeTable)
            .SetTerrainPoints(this.terrainPoints.ToArray())
            .SetOceanPoints(this.oceanPoints.ToArray())
            .SetOceanSizeMultiplier(this.oceanSizeMultiplier)
            .End();
    }
}