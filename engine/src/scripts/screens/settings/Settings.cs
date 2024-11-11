
using Godot;


public partial class Settings : Control
{
    [Export] 
    public bool ShowBackground = true;
    [Export] 
    public bool InsideSimulation = false;
    
    [ExportGroup("DO NOT EDIT THESE")]
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
    [Export] 
    public Button BackButton;
    [Export] 
    public DialogConfirm ConfirmDialog;
    
    [Export] 
    public ColorRect Background;

    public override void _Ready()
    {
        this.SaveButton.Pressed += this.OnSaveButtonPressed;
        this.ResetButton.Pressed += this.OnResetButtonPressed;
        this.BackButton.Pressed += this.OnBackButtonPressed;

        this.ConfirmDialog.Confirmed += this.OnConfirmDialogClick;

        this.Background.Visible = this.ShowBackground;
    }

    private void OnSaveButtonPressed()
    {
        this.DisplaySettings.ApplySettings();
        this.ControlsSettings.ApplySettings();
        this.SaveSettings.ApplySettings();
        EventManager.Instance.RegisterEvent(new NotifyEvent(null), EventChannel.Settings);
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
            this.ConfirmDialog.Open();
        }
        else
        {
            this.Return();
        }
    }

    private void OnConfirmDialogClick()
    {
        this.Return();
    }

    private void Return()
    {
        if (this.InsideSimulation)
        {
            this.Visible = false;
        }
        else
        {
            this.GetTree().ChangeSceneToFile("res://src/scenes/mainMenu/mainMenu.tscn");
        }
    }

    private bool HasUnsavedChanges()
    {
        return this.DisplaySettings.HasUnsavedChanges() 
               || this.ControlsSettings.HasUnsavedChanges() 
               || this.SaveSettings.HasUnsavedChanges();
    }
}
