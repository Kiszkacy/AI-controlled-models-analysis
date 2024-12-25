using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Godot;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using FileAccess = Godot.FileAccess;

public partial class SelectedSimulation : Panel
{
    [ExportGroup("DO NOT EDIT THESE")]
    [Export] public Button FileManagerButton;
    [Export] public Button CloseButton;
    [Export] public TextureRect SimulationScreenshot;
    [Export] public Button SettingsButton;
    [Export] public Button DeleteButton;
    [Export] public Button RunButton;
    [Export] public Label NameLabel;
    [Export] public Label DateTimeLabel;
    [Export] public Label AgentCountLabel;
    [Export] public Label TimeRunningLabel;
    [Export] public FileDialog FileDialogWindow;
    [Export] public DialogConfirm ConfirmDialog;

    private String simulationPath;
    private Timer fadeinTimer;
    private Timer fadeoutTimer;
    private float fadeDuration = 0.3f; // in seconds
    public Action SimulationDeleted;
    public Action SimulationClosed;
    public int nameCharacterLimit { get; private set; } = 13;


    public override void _Ready()
    {
        this.ConnectButtons();

        this.FileDialogWindow.FileMode = FileDialog.FileModeEnum.OpenFile;
        this.FileDialogWindow.Access = FileDialog.AccessEnum.Filesystem;
        this.FileDialogWindow.Filters = new[] { "*.gsave" };
        this.FileDialogWindow.GetOkButton().Disabled = true;
        this.FileDialogWindow.GetCancelButton().Disabled = true;

        this.fadeinTimer = new(this.OnFadeinTimeout);
        this.fadeoutTimer = new(this.OnFadeoutTimeout);
    }

    private void ConnectButtons()
    {
        this.FileManagerButton.Pressed += this.OnFileManagerButtonPressed;
        this.CloseButton.Pressed += this.OnCloseButtonPressed;
        this.SettingsButton.Pressed += this.OnSettingsButtonPressed;
        this.DeleteButton.Pressed += this.OnDeleteButtonPressed;
        this.RunButton.Pressed += this.OnRunButtonPressed;
        this.ConfirmDialog.Confirmed += this.OnConfirmDialogClick;
    }

    public void ShowSimulation(string simulationPath)
    {
        if (!FileAccess.FileExists(simulationPath) || !simulationPath.EndsWith(".gsave"))
        {
            NeatPrinter.Start()
                .ColorPrint(ConsoleColor.Red, "[SELECTED SIMULATION]")
                .Print("  | ERROR: Invalid simulation path.")
                .End();
            return;
        }

        this.simulationPath = simulationPath;
        this.FileDialogWindow.CurrentPath = ProjectSettings.GlobalizePath(simulationPath);

        this.SetDisplayValues();
        this.Visible = true;
        this.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        if (this.SimulationScreenshot != null && this.SimulationScreenshot.Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("external_alpha", 0.0f);
        }
        this.fadeinTimer.Activate(this.fadeDuration);
    }

    private void SetDisplayValues()
    {
        string[] pathParts = Path.GetFileNameWithoutExtension(this.simulationPath).Split('-');
        string simulationName = pathParts[0];
        SetNameLabel(simulationName);
        string dateTime = string.Join("-", pathParts.Skip(1));
        DateTime parsedDateTime = DateTime.ParseExact(
            dateTime,
            "yyyy-MM-ddTHH-mm-ss",
            CultureInfo.InvariantCulture
        );
        this.DateTimeLabel.Text = parsedDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        FileAccess file = FileAccess.Open(this.simulationPath, FileAccess.ModeFlags.Read);
        string yaml = file.GetAsText();
        file.Close();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        EnvironmentSaveData environmentSaveData = deserializer.Deserialize<EnvironmentSaveData>(yaml);

        this.AgentCountLabel.Text = environmentSaveData.AllAgentsData.Length.ToString();
        TimeSpan timeSpan = TimeSpan.FromSeconds(environmentSaveData.SimulationStatusData.TimePassed);
        this.TimeRunningLabel.Text = timeSpan.ToString(@"hh\:mm\:ss");

        this.SetScreenshotTexture();
    }

    private void SetNameLabel(string simulationName)
    {
        this.NameLabel.Text = simulationName.Length > this.nameCharacterLimit
            ? simulationName.Substring(0, this.nameCharacterLimit) + "..."
            : simulationName;
    }

    private void SetScreenshotTexture()
    {
        string screenshotPath = Path.ChangeExtension(this.simulationPath, ".png");
        if (FileAccess.FileExists(screenshotPath))
        {
            var image = new Image();
            Error err = image.Load(screenshotPath);
            if (err == Error.Ok)
            {
                Vector2 rectSize = this.SimulationScreenshot.Size;
                image.Resize((int)rectSize.X, (int)rectSize.Y);
                image.Convert(Image.Format.Rgba8);
                var imageTexture = ImageTexture.CreateFromImage(image);
                this.SimulationScreenshot.Texture = imageTexture;
            }
        }
        else
        {
            NeatPrinter.Start()
                .ColorPrint(ConsoleColor.Red, "[SELECTED SIMULATION]")
                .Print("  | ERROR: Screenshot not found.")
                .End();
        }
    }

    public override void _Process(double delta)
    {
        if (this.fadeinTimer.IsActive)
        {
            this.fadeinTimer.Process(delta);
            this.Modulate =  new Color(1.0f, 1.0f, 1.0f, (float)(1.0f - this.fadeinTimer.Time/this.fadeDuration));
            if (this.SimulationScreenshot != null && this.SimulationScreenshot.Material is ShaderMaterial shaderMaterial)
            {
                shaderMaterial.SetShaderParameter("external_alpha", (float)(1.0f - this.fadeinTimer.Time/this.fadeDuration));
            }
        }
        else if (this.fadeoutTimer.IsActive)
        {
            this.fadeoutTimer.Process(delta);
            this.Modulate =  new Color(1.0f, 1.0f, 1.0f, (float)(this.fadeoutTimer.Time/this.fadeDuration));
            if (this.SimulationScreenshot != null && this.SimulationScreenshot.Material is ShaderMaterial shaderMaterial)
            {
                shaderMaterial.SetShaderParameter("external_alpha", (float)(this.fadeoutTimer.Time/this.fadeDuration));
            }
        }
    }

    private void OnFileManagerButtonPressed()
    {
        Vector2I size = (Vector2I)(GetViewport().GetVisibleRect().Size * 0.8f);
        this.FileDialogWindow.PopupCentered(size);
    }

    private void OnCloseButtonPressed()
    {
        this.fadeoutTimer.Activate(this.fadeDuration);
        SimulationClosed?.Invoke();
    }

    private void OnSettingsButtonPressed()
    {
        // TODO
    }

    private void OnDeleteButtonPressed()
    {
        this.ConfirmDialog.Open();
    }

    private void OnRunButtonPressed()
    {
        Node root = GetTree().Root;
        NeatPrinter.Start()
            .Print("  | LOADING SELECTED SIMULATION")
            .End();
        Reloader.Get().LoadSimulation(root, simulationPath, true);
    }

    private void OnConfirmDialogClick()
    {
        string absolutePath = ProjectSettings.GlobalizePath(simulationPath);

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
            NeatPrinter.Start()
                .Print("Deleted simulation at " + simulationPath + ".")
                .End();
            SimulationDeleted?.Invoke();
        }
        else
        {
            NeatPrinter.Start()
                .ColorPrint(ConsoleColor.Red, "Error. There is no simulation at " + simulationPath + ".")
                .End();
        }

        this.fadeoutTimer.Activate(this.fadeDuration);

        string screenshotPath = Path.ChangeExtension(this.simulationPath, ".png");
        string absoluteScreenshotPath = ProjectSettings.GlobalizePath(screenshotPath);
        if (File.Exists(absoluteScreenshotPath))
        {
            File.Delete(absoluteScreenshotPath);
        }
    }

    private void OnFadeinTimeout()
    {
    }

    private void OnFadeoutTimeout()
    {
        this.Visible = false;
    }

}