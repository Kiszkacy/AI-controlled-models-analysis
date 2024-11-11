
using System.IO;

using Godot;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public partial class DisplaySettings : Control
{
    [Export] public Button ResolutionArrowLeft;
    [Export] public Button ResolutionArrowRight;
    [Export] public Label ResolutionLabel;
    [Export] public Button ModeArrowLeft;
    [Export] public Button ModeArrowRight;
    [Export] public Label ModeLabel;
    [Export] public Button LockMouseArrowLeft;
    [Export] public Button LockMouseArrowRight;
    [Export] public Label LockMouseLabel;

    private readonly string CONFIG_PATH = "./src/config.yaml";

    private readonly Vector2I[] availableResolutions = {
        new(1280, 720),
        new(1920, 1080),
        new(2560, 1440),
        new(3840, 2160)
    };

    private int currentResolutionIndex = 1;
    private bool isFullscreen = true;
    private bool isMouseLocked = true;

    public override void _Ready()
    {
        this.ResolutionArrowLeft.Pressed += this.OnResolutionArrowLeftPressed;
        this.ResolutionArrowRight.Pressed += this.OnResolutionArrowRightPressed;
        this.ModeArrowLeft.Pressed += this.OnModeArrowLeftPressed;
        this.ModeArrowRight.Pressed += this.OnModeArrowRightPressed;
        this.LockMouseArrowLeft.Pressed += this.OnLockMouseArrowLeftPressed;
        this.LockMouseArrowRight.Pressed += this.OnLockMouseArrowRightPressed;

        LoadCurrentSettings();
        UpdateUI();
    }

    private void LoadCurrentSettings()
    {
        var windowMode = DisplayServer.WindowGetMode();
        this.isFullscreen = windowMode == DisplayServer.WindowMode.Fullscreen ||
                            windowMode == DisplayServer.WindowMode.ExclusiveFullscreen;
        this.isMouseLocked = Input.GetMouseMode() == Input.MouseModeEnum.Captured;
        Vector2I currentResolution = DisplayServer.WindowGetSize();
        var closestDiff = float.MaxValue;
        for (int i = 0; i < this.availableResolutions.Length; i++)
        {
            Vector2I resolution = this.availableResolutions[i];
            float diff = Mathf.Abs(resolution.X - currentResolution.X) + Mathf.Abs(resolution.Y - currentResolution.Y);
            if (diff < closestDiff)
            {
                this.currentResolutionIndex = i;
                closestDiff = diff;
            }
        }
    }

    private void UpdateUI()
    {
        ResolutionLabel.Text = $"{this.availableResolutions[this.currentResolutionIndex].X}x{this.availableResolutions[this.currentResolutionIndex].Y}";
        ModeLabel.Text = this.isFullscreen ? "Fullscreen" : "Windowed";
        LockMouseLabel.Text = this.isMouseLocked ? "Yes" : "No";
    }

    private void OnResolutionArrowLeftPressed()
    {
        this.currentResolutionIndex = Mathf.Max(this.currentResolutionIndex - 1, 0);
        UpdateUI();
    }

    private void OnResolutionArrowRightPressed()
    {
        this.currentResolutionIndex = Mathf.Min(this.currentResolutionIndex + 1, this.availableResolutions.Length - 1);
        UpdateUI();
    }

    private void OnModeArrowLeftPressed()
    {
        this.isFullscreen = !this.isFullscreen;
        UpdateUI();
    }

    private void OnModeArrowRightPressed()
    {
        this.isFullscreen = !this.isFullscreen;
        UpdateUI();
    }

    private void OnLockMouseArrowLeftPressed()
    {
        this.isMouseLocked = !this.isMouseLocked;
        UpdateUI();
    }

    private void OnLockMouseArrowRightPressed()
    {
        this.isMouseLocked = !this.isMouseLocked;
        UpdateUI();
    }

    public void ApplySettings()
    {
        if (this.isFullscreen)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            DisplayServer.WindowSetSize(this.availableResolutions[this.currentResolutionIndex]);
        }
        Input.SetMouseMode(this.isMouseLocked ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible);
        SaveSettingsToConfig();
    }

    public void ResetToDefault()
    {
        this.currentResolutionIndex = 1;
        this.isFullscreen = true;
        this.isMouseLocked = true;
        UpdateUI();
    }

    private void SaveSettingsToConfig()
    {
        var yaml = File.ReadAllText(CONFIG_PATH);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var configDict = deserializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(yaml);

        var displayData = new DisplayData(new Vector2I(this.availableResolutions[this.currentResolutionIndex].X, this.availableResolutions[this.currentResolutionIndex].Y),
            this.isFullscreen, this.isMouseLocked);

        configDict["display"] = displayData;

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var updatedYaml = serializer.Serialize(configDict);

        File.WriteAllText(CONFIG_PATH, updatedYaml);

        Config.Instance.Display.Resolution = this.availableResolutions[this.currentResolutionIndex];
        Config.Instance.Display.IsFullscreen = this.isFullscreen;
        Config.Instance.Display.IsMouseLocked = this.isMouseLocked;
    }

}

public class DisplayData
{
    public Vector2I Resolution { get; set; }
    public bool IsFullscreen { get; set; }
    public bool IsMouseLocked { get; set; }

    public DisplayData(Vector2I resolution, bool isFullscreen, bool isMouseLocked)
    {
        this.Resolution = resolution;
        this.IsFullscreen = isFullscreen;
        this.IsMouseLocked = isMouseLocked;
    }

    public DisplayData() { }
}