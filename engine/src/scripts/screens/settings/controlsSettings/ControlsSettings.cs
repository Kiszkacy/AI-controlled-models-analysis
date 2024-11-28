
using System.IO;
using System.Linq;

using Godot;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public partial class ControlsSettings : Control
{
    [Export] public HSlider ZoomSensitivitySlider;
    [Export] public Button MaxZoomArrowLeft;
    [Export] public Button MaxZoomArrowRight;
    [Export] public Label MaxZoomLabel;
    [Export] public Button MinZoomArrowLeft;
    [Export] public Button MinZoomArrowRight;
    [Export] public Label MinZoomLabel;
    [Export] public CheckButton EdgeMoveEnabled;
    [Export] public Button EdgeMoveMarginButton;
    [Export] public Button EdgeMoveMarginArrowLeft;
    [Export] public Button EdgeMoveMarginArrowRight;
    [Export] public Label EdgeMoveMarginLabel;
    [Export] public Button EdgeMoveSpeedButton;
    [Export] public HSlider EdgeMoveSpeedSlider;

    private readonly double[] availableMaxZooms = new double[9];
    private readonly double[] availableMinZooms = new double[10];
    private readonly float[] availableEdgeMoveMargins = new float[10];

    private int currentMaxZoomIndex = 4;
    private int currentMinZoomIndex = 2;
    private int currentEdgeMoveMarginIndex = 1;
    private double currentZoomSensitivity = 1.0;
    private bool isEdgeMoveEnabled = true;
    private double currentEdgeMoveSpeedQuantifier = 3.0;

    private int initialMaxZoomIndex;
    private int initialMinZoomIndex;
    private int initialEdgeMoveMarginIndex;
    private double initialZoomSensitivity;
    private bool initialEdgeMoveEnabled;
    private double initialEdgeMoveSpeedQuantifier;

    private readonly string configPath = "./src/config.yaml";

    public override void _Ready()
    {
        for (int i = 0; i < this.availableMaxZooms.Length; i++)
        {
            this.availableMaxZooms[i] = System.Math.Round(1.0 + i * 0.5, 1);
        }
        for (int i = 0; i < this.availableMinZooms.Length; i++)
        {
            this.availableMinZooms[i] = System.Math.Round(0.1 + i * 0.1, 1);
        }
        for (int i = 0; i < this.availableEdgeMoveMargins.Length; i++)
        {
            this.availableEdgeMoveMargins[i] = 10.0f + i * 10.0f;
        }

        LoadFromConfig();
        SetSliders();
        
        this.EdgeMoveEnabled.ButtonPressed = isEdgeMoveEnabled;
        
        ConnectButtons();
        
        StoreInitialState();
        UpdateUI();
    }

    private void StoreInitialState()
    {
        initialMaxZoomIndex = currentMaxZoomIndex;
        initialMinZoomIndex = currentMinZoomIndex;
        initialEdgeMoveMarginIndex = currentEdgeMoveMarginIndex;
        initialZoomSensitivity = currentZoomSensitivity;
        initialEdgeMoveEnabled = isEdgeMoveEnabled;
        initialEdgeMoveSpeedQuantifier = currentEdgeMoveSpeedQuantifier;
    }

    public bool HasUnsavedChanges()
    {
        return initialMaxZoomIndex != currentMaxZoomIndex ||
               initialMinZoomIndex != currentMinZoomIndex ||
               initialEdgeMoveMarginIndex != currentEdgeMoveMarginIndex ||
               !Mathf.IsEqualApprox((float)initialZoomSensitivity, (float)currentZoomSensitivity) ||
               initialEdgeMoveEnabled != isEdgeMoveEnabled ||
               !Mathf.IsEqualApprox((float)initialEdgeMoveSpeedQuantifier, (float)currentEdgeMoveSpeedQuantifier);
    }

    private void SetSliders()
    {
        this.ZoomSensitivitySlider.MinValue = 0.1;
        this.ZoomSensitivitySlider.MaxValue = 2.0;
        this.ZoomSensitivitySlider.Step = 0.1;
        this.ZoomSensitivitySlider.Value = Mathf.Clamp(
            this.currentZoomSensitivity,
            this.ZoomSensitivitySlider.MinValue,
            this.ZoomSensitivitySlider.MaxValue
        );

        this.EdgeMoveSpeedSlider.MinValue = 0.5;
        this.EdgeMoveSpeedSlider.MaxValue = 10.0;
        this.EdgeMoveSpeedSlider.Step = 0.1;
        this.EdgeMoveSpeedSlider.Value = Mathf.Clamp(
            this.currentEdgeMoveSpeedQuantifier,
            this.EdgeMoveSpeedSlider.MinValue,
            this.EdgeMoveSpeedSlider.MaxValue
        );
    }

    private void ConnectButtons()
    {
        this.ZoomSensitivitySlider.ValueChanged += this.OnZoomSensitivitySliderValueChanged;
        this.MaxZoomArrowLeft.Pressed += this.OnMaxZoomArrowLeftPressed;
        this.MaxZoomArrowRight.Pressed += this.OnMaxZoomArrowRightPressed;
        this.MinZoomArrowLeft.Pressed += this.OnMinZoomArrowLeftPressed;
        this.MinZoomArrowRight.Pressed += this.OnMinZoomArrowRightPressed;
        this.EdgeMoveEnabled.Pressed += this.OnEdgeMoveEnabledPressed;
        this.EdgeMoveMarginArrowLeft.Pressed += this.OnEdgeMoveMarginArrowLeftPressed;
        this.EdgeMoveMarginArrowRight.Pressed += this.OnEdgeMoveMarginArrowRightPressed;
        this.EdgeMoveSpeedSlider.ValueChanged += this.OnEdgeMoveSpeedSliderValueChanged;
    }

    private void UpdateUI()
    {
        this.MaxZoomLabel.Text = this.availableMaxZooms[this.currentMaxZoomIndex].ToString();
        this.MinZoomLabel.Text = this.availableMinZooms[this.currentMinZoomIndex].ToString();
        this.EdgeMoveMarginLabel.Text = $"{this.availableEdgeMoveMargins[this.currentEdgeMoveMarginIndex]} px";
        
        if (!this.isEdgeMoveEnabled)
        {
            this.EdgeMoveMarginButton.Disabled = true;
            this.EdgeMoveMarginArrowLeft.Disabled = true;
            this.EdgeMoveMarginArrowRight.Disabled = true;
            this.EdgeMoveSpeedButton.Disabled = true;
            this.EdgeMoveSpeedSlider.Editable = false;
        }
        else
        {
            this.EdgeMoveMarginButton.Disabled = false;
            this.EdgeMoveMarginArrowLeft.Disabled = false;
            this.EdgeMoveMarginArrowRight.Disabled = false;
            this.EdgeMoveSpeedButton.Disabled = false;
            this.EdgeMoveSpeedSlider.Editable = true;
        }
    }

    private void OnZoomSensitivitySliderValueChanged(double value)
    {
        this.currentZoomSensitivity = value;
    }

    private void OnMaxZoomArrowLeftPressed()
    {
        if (this.currentMaxZoomIndex > 0)
        {
            this.currentMaxZoomIndex--;
            UpdateUI();
        }
    }

    private void OnMaxZoomArrowRightPressed()
    {
        if (this.currentMaxZoomIndex < this.availableMaxZooms.Length - 1)
        {
            this.currentMaxZoomIndex++;
            UpdateUI();
        }
    }

    private void OnMinZoomArrowLeftPressed()
    {
        if (this.currentMinZoomIndex > 0)
        {
            this.currentMinZoomIndex--;
            UpdateUI();
        }
    }

    private void OnMinZoomArrowRightPressed()
    {
        if (this.currentMinZoomIndex < this.availableMinZooms.Length - 1)
        {
            this.currentMinZoomIndex++;
            UpdateUI();
        }
    }

    private void OnEdgeMoveEnabledPressed()
    {
        this.isEdgeMoveEnabled = !this.isEdgeMoveEnabled;
        UpdateUI();
    }

    private void OnEdgeMoveMarginArrowLeftPressed()
    {
        if (this.currentEdgeMoveMarginIndex > 0)
        {
            this.currentEdgeMoveMarginIndex--;
            UpdateUI();
        }
    }

    private void OnEdgeMoveMarginArrowRightPressed()
    {
        if (this.currentEdgeMoveMarginIndex < this.availableEdgeMoveMargins.Length - 1)
        {
            this.currentEdgeMoveMarginIndex++;
            UpdateUI();
        }
    }

    private void OnEdgeMoveSpeedSliderValueChanged(double value)
    {
        this.currentEdgeMoveSpeedQuantifier = value;
    }

    public void ResetToDefault()
    {
        currentMaxZoomIndex = 4;
        currentMinZoomIndex = 2;
        currentEdgeMoveMarginIndex = 1;
        currentZoomSensitivity = 1.0;
        ZoomSensitivitySlider.Value = currentZoomSensitivity;
        isEdgeMoveEnabled = true;
        currentEdgeMoveSpeedQuantifier = 3.0;
        EdgeMoveSpeedSlider.Value = currentEdgeMoveSpeedQuantifier;
        UpdateUI();
    }

    public void ApplySettings()
    {
        this.initialZoomSensitivity = this.currentZoomSensitivity;
        this.initialMaxZoomIndex = this.currentMaxZoomIndex;
        this.initialMinZoomIndex = this.currentMinZoomIndex;
        this.initialEdgeMoveMarginIndex = this.currentEdgeMoveMarginIndex;
        this.initialEdgeMoveEnabled = this.isEdgeMoveEnabled;
        this.initialEdgeMoveSpeedQuantifier = this.currentEdgeMoveSpeedQuantifier;
        SaveSettingsToConfig();
    }

    private void SaveSettingsToConfig()
    {
        var yaml = File.ReadAllText(configPath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var configDict = deserializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(yaml);

        var controlsData = new ControlsData(
            (float)this.currentZoomSensitivity,
            (float)this.availableMaxZooms[this.currentMaxZoomIndex],
            (float)this.availableMinZooms[this.currentMinZoomIndex],
            this.isEdgeMoveEnabled,
            this.availableEdgeMoveMargins[this.currentEdgeMoveMarginIndex],
            (float)this.currentEdgeMoveSpeedQuantifier
        );

        configDict["controls"] = controlsData;
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var updatedYaml = serializer.Serialize(configDict);

        File.WriteAllText(configPath, updatedYaml);

        Config.Instance.Controls.ZoomSensitivity = (float)this.currentZoomSensitivity;
        Config.Instance.Controls.MaxZoom = (float)this.availableMaxZooms[this.currentMaxZoomIndex];
        Config.Instance.Controls.MinZoom = (float)this.availableMinZooms[this.currentMinZoomIndex];
        Config.Instance.Controls.EdgeMoveEnabled = this.isEdgeMoveEnabled;
        Config.Instance.Controls.EdgeMoveMargin = this.availableEdgeMoveMargins[this.currentEdgeMoveMarginIndex];
        Config.Instance.Controls.EdgeMoveSpeed = (float)this.currentEdgeMoveSpeedQuantifier;
    }

    private void LoadFromConfig()
    {
        this.currentZoomSensitivity = System.Math.Round(Config.Instance.Data.Controls.ZoomSensitivity, 1);
        this.currentMaxZoomIndex = System.Array.IndexOf(availableMaxZooms, 
            availableMaxZooms.MinBy(zoom => Mathf.Abs(zoom - Config.Instance.Data.Controls.MaxZoom)));
        this.currentMinZoomIndex = System.Array.IndexOf(availableMinZooms, 
            availableMinZooms.MinBy(zoom => Mathf.Abs(zoom - Config.Instance.Data.Controls.MinZoom)));
        this.isEdgeMoveEnabled = Config.Instance.Data.Controls.EdgeMoveEnabled;
        this.currentEdgeMoveMarginIndex = System.Array.IndexOf(this.availableEdgeMoveMargins, Config.Instance.Data.Controls.EdgeMoveMargin);
        this.currentEdgeMoveSpeedQuantifier = System.Math.Round(Config.Instance.Data.Controls.EdgeMoveSpeed, 1);
    }
}

public class ControlsData
{
    public float ZoomSensitivity { get; set; }
    public float MaxZoom { get; set; }
    public float MinZoom { get; set; }
    public bool EdgeMoveEnabled { get; set; }
    public float EdgeMoveMargin { get; set; }
    public float EdgeMoveSpeed { get; set; }

    public ControlsData(float zoomSensitivity, float maxZoom, float minZoom, bool edgeMoveEnabled, float edgeMoveMargin, float edgeMoveSpeed)
    {
        ZoomSensitivity = zoomSensitivity;
        MaxZoom = maxZoom;
        MinZoom = minZoom;
        EdgeMoveEnabled = edgeMoveEnabled;
        EdgeMoveMargin = edgeMoveMargin;
        EdgeMoveSpeed = edgeMoveSpeed;
    }

    public ControlsData() { }
}
