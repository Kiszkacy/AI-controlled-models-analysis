using System;

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

    private String simulationPath;
    
    public override void _Ready()
    {
        this.FileManagerButton.Pressed += this.OnFileManagerButtonPressed;
        this.CloseButton.Pressed += this.OnCloseButtonPressed;
        this.SettingsButton.Pressed += this.OnSettingsButtonPressed;
        this.DeleteButton.Pressed += this.OnDeleteButtonPressed;
        this.RunButton.Pressed += this.OnRunButtonPressed;
        
        this.FileDialogWindow.FileMode = FileDialog.FileModeEnum.OpenFile;
        this.FileDialogWindow.CurrentPath = simulationPath;
        this.FileDialogWindow.GetOkButton().Disabled = true;
        this.FileDialogWindow.GetCancelButton().Disabled = true;
    }

    private void OnFileManagerButtonPressed()
    {
        Vector2I size = (Vector2I)(GetViewport().GetVisibleRect().Size * 0.8f);
        this.FileDialogWindow.PopupCentered(size);
    }

}