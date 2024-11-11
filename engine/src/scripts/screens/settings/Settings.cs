
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
    public override void _Ready()
    {
        this.SaveButton.Pressed += this.OnSaveButtonPressed;
        this.ResetButton.Pressed += this.OnResetButtonPressed;
    }

    private void OnSaveButtonPressed()
    {
        switch (this.TabContainer.CurrentTab)
        {
            case 0:
                this.DisplaySettings.ApplySettings();
                break;
            case 1:
                this.ControlsSettings.ApplySettings();
                break;
            case 2:
                this.SaveSettings.ApplySettings();
                break;
        }
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


}