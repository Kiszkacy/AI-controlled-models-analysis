using System;
using System.IO;

using Godot;

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
    private Timer fadeoutTimer;
    private float fadeoutDuration = 1.0f; // in seconds
    
    public override void _Ready()
    {
        this.ConnectButtons();
        
        this.FileDialogWindow.FileMode = FileDialog.FileModeEnum.OpenFile;
        this.FileDialogWindow.CurrentPath = simulationPath;
        this.FileDialogWindow.GetOkButton().Disabled = true;
        this.FileDialogWindow.GetCancelButton().Disabled = true;
        
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
    
    public override void _Process(double delta)
    {
        if (this.fadeoutTimer.IsActive)
        {
            this.fadeoutTimer.Process(delta);
            this.Modulate =  new Color(1.0f, 1.0f, 1.0f, (float)(this.fadeoutTimer.Time/this.fadeoutDuration));
            this.SimulationScreenshot.Modulate =  new Color(1.0f, 1.0f, 1.0f, (float)(this.fadeoutTimer.Time/this.fadeoutDuration));
        }
    }

    private void OnFileManagerButtonPressed()
    {
        Vector2I size = (Vector2I)(GetViewport().GetVisibleRect().Size * 0.8f);
        this.FileDialogWindow.PopupCentered(size);
    }
    
    private void OnCloseButtonPressed()
    {
        this.fadeoutTimer.Activate(this.fadeoutDuration);
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
        
        this.fadeoutTimer.Activate(this.fadeoutDuration);
        
        // TODO: delete screenshot also
    }

    private void OnFadeoutTimeout()
    {
        this.Visible = false;
    }

}
