using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Godot;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using FileAccess = Godot.FileAccess;

public partial class SelectedSimulation: Panel
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
    private float fadeDuration = 1.0f; // in seconds
    
    
    public override void _Ready()
    {
        this.ConnectButtons();
        
        this.FileDialogWindow.FileMode = FileDialog.FileModeEnum.OpenFile;
        this.FileDialogWindow.CurrentPath = simulationPath;
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
        this.SetDisplayValues();
        this.fadeinTimer.Activate(this.fadeDuration);
    }
    
    private void SetDisplayValues()
    {
        string[] pathParts = Path.GetFileNameWithoutExtension(this.simulationPath).Split('-');
        string simulationName = pathParts[0];
        this.NameLabel.Text = simulationName;
        string dateTime = string.Join("-", pathParts.Skip(1));
        DateTime parsedDateTime = DateTime.ParseExact(
            dateTime.Replace('_', ' '), 
            "yyyy-MM-dd HH-mm-ss", 
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

    private void SetScreenshotTexture()
    {
        string screenshotPath = Path.ChangeExtension(this.simulationPath, ".png");
        
        if (File.Exists(screenshotPath))
        {
            var texture = ResourceLoader.Load<Texture2D>(screenshotPath);
            this.SimulationScreenshot.Texture = texture;
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
            this.SimulationScreenshot.Modulate =  new Color(1.0f, 1.0f, 1.0f, (float)(1.0f - this.fadeinTimer.Time/this.fadeDuration));
        }
        else if (this.fadeoutTimer.IsActive)
        {
            this.fadeoutTimer.Process(delta);
            this.Modulate =  new Color(1.0f, 1.0f, 1.0f, (float)(this.fadeoutTimer.Time/this.fadeDuration));
            this.SimulationScreenshot.Modulate =  new Color(1.0f, 1.0f, 1.0f, (float)(this.fadeoutTimer.Time/this.fadeDuration));
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
        Node root = GetTree().Root.GetChild(0);
        NeatPrinter.Start()
            .Print("  | LOADING SELECTED SIMULATION")
            .End();
        Reloader.Get().LoadSimulation(root, simulationPath);
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
