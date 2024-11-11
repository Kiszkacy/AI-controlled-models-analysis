
using Godot;


public partial class Settings : Control
{
    [Export]
    public TabContainer TabContainer;
    [Export]
    public DisplaySettings DisplaySettings;
    [Export]
    public ControlsSettings ControlsSettings;
    [Export]
    public SaveSettings SaveSettings;
    [Export]
    public Button SaveButton;
    [Export]
    public Button ResetButton;
    [Export] public Button BackButton;
    [Export] public ConfirmationDialog ConfirmDialog;

    public override void _Ready()
    {
        this.SaveButton.Pressed += this.OnSaveButtonPressed;
        this.ResetButton.Pressed += this.OnResetButtonPressed;
        this.BackButton.Pressed += this.OnBackButtonPressed;

        ConfirmDialog.GetOkButton().Pressed += this.LoadMainScene;
    }

    private void OnSaveButtonPressed()
    {
        this.DisplaySettings.ApplySettings();
        this.ControlsSettings.ApplySettings();
        this.SaveSettings.ApplySettings();
    }

    private void OnResetButtonPressed()
    {
        switch (this.TabContainer.CurrentTab)
        {
            case 0:
                this.DisplaySettings.ResetToDefault();
                break;
            case 1:
                this.ControlsSettings.ResetToDefault();
                break;
            case 2:
                this.SaveSettings.ResetToDefault();
                break;
        }
    }

    private void OnBackButtonPressed()
    {
        if (HasUnsavedChanges())
        {
            ConfirmDialog.PopupCentered();
        }
        else
        {
            LoadMainScene();
        }
    }

    private void LoadMainScene()
    {
        PackedScene mainScene = ResourceLoader.Load<PackedScene>("res://src/scenes/simulation/simulation.tscn");
        GetTree().ChangeSceneToPacked(mainScene);
    }

    private bool HasUnsavedChanges()
    {
        return this.DisplaySettings.HasUnsavedChanges() ||
               this.ControlsSettings.HasUnsavedChanges() ||
               this.SaveSettings.HasUnsavedChanges();
    }
}