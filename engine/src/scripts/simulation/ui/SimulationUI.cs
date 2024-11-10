using System;

using Godot;

public enum ViewMode
{
    Simulation,
    Analysis,
    Configuration
}


public partial class SimulationUI : CanvasLayer
{
    [Export]
    public Button ExitButton;
    [Export]
    public Button SettingsButton;
    [Export]
    public PauseButton PauseButton;
    [Export]
    public Control PauseNode;
    [Export]
    public Button SimulationSpeedLevelOneButton;
    [Export]
    public Button SimulationSpeedLevelTwoButton;
    [Export]
    public Button SimulationSpeedLevelThreeButton;
    [Export]
    public Button SimulationSpeedLevelFourButton;
    [Export]
    public Label SimulationTimeLabel;
    [Export]
    public Label SystemTimeLabel;
    [Export]
    public Button CenterCameraButton;
    [Export]
    public Camera Camera;
    [Export]
    public Control SubmenuNode;
    [Export]
    public ColorRect FadeoutBackground;

    [Export]
    public ButtonHandler SimulationViewButton;
    [Export]
    public ButtonHandler AnalysisViewButton;
    [Export]
    public ButtonHandler ConfigurationViewButton;
    
    [Export]
    public Control SimulationViewNode;
    [Export]
    public Control AnalysisViewNode;
    [Export]
    public Control ConfigurationViewNode;

    private ViewMode ViewMode = ViewMode.Simulation;

    private readonly int FadeoutBackgroundZIndex = 2;
    private readonly int FadeoutBackgroundZIndexWhenNested = 4;

    private bool isSubmenuVisible => this.SubmenuNode.Visible;

    private const float levelOneSimulationSpeed = 1.0f;
    private const float levelTwoSimulationSpeed = 2.0f;
    private const float levelThreeSimulationSpeed = 3.0f;
    private const float levelFourSimulationSpeed = 4.0f;

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("escape") && !isSubmenuVisible)
        {
            this.OnSettingsClick();
        }
        else if (@event.IsActionPressed("escape") && isSubmenuVisible)
        {
            this.FadeoutBackground.Visible = this.ViewMode != ViewMode.Simulation;
            this.FadeoutBackground.ZIndex = this.FadeoutBackgroundZIndex;
            this.SubmenuNode.Visible = false;
            SimulationManager.Instance.Resume(this);
        }
    }

    public override void _Ready()
    {
        this.ConnectButtons();
        this.UpdateViewButtonsState();
        this.OnSimulationSpeedLevelOneClick();
    }

    private void ConnectButtons()
    {
        this.ExitButton.Pressed += this.OnExitClick;
        this.SettingsButton.Pressed += this.OnSettingsClick;
        this.PauseButton.Pressed += this.OnPauseClick;
        this.SimulationSpeedLevelOneButton.Pressed += this.OnSimulationSpeedLevelOneClick;
        this.SimulationSpeedLevelTwoButton.Pressed += this.OnSimulationSpeedLevelTwoClick;
        this.SimulationSpeedLevelThreeButton.Pressed += this.OnSimulationSpeedLevelThreeClick;
        this.SimulationSpeedLevelFourButton.Pressed += this.OnSimulationSpeedLevelFourClick;
        this.SimulationViewButton.Pressed += this.OnSimulationViewClick;
        this.AnalysisViewButton.Pressed += this.OnAnalysisViewClick;
        this.ConfigurationViewButton.Pressed += this.OnConfigurationViewClick;
        this.CenterCameraButton.Pressed += this.OnCenterCameraClick;
    }

    private void OnExitClick()
    {
        this.GetTree().Quit();
    }

    private void OnSettingsClick()
    {
        this.FadeoutBackground.Visible = true;
        this.FadeoutBackground.ZIndex = this.FadeoutBackgroundZIndexWhenNested;
        this.SubmenuNode.Visible = true;
        SimulationManager.Instance.Pause(this);
    }

    private void OnPauseClick()
    {
        if (SimulationManager.Instance.IsSimulationPaused)
        {
            SimulationManager.Instance.Resume(this);
        }
        else
        {
            SimulationManager.Instance.Pause(this);
        }

        this.UpdatePauseLabelVisibility();
        this.PauseButton.Invert();
    }

    private void OnSimulationSpeedLevelOneClick()
    {
        Engine.TimeScale = levelOneSimulationSpeed;
        this.SimulationSpeedLevelOneButton.SetPressed(true);
        this.SimulationSpeedLevelTwoButton.SetPressed(false);
        this.SimulationSpeedLevelThreeButton.SetPressed(false);
        this.SimulationSpeedLevelFourButton.SetPressed(false);
    }

    private void OnSimulationSpeedLevelTwoClick()
    {
        Engine.TimeScale = levelTwoSimulationSpeed;
        this.SimulationSpeedLevelOneButton.SetPressed(true);
        this.SimulationSpeedLevelTwoButton.SetPressed(true);
        this.SimulationSpeedLevelThreeButton.SetPressed(false);
        this.SimulationSpeedLevelFourButton.SetPressed(false);
    }

    private void OnSimulationSpeedLevelThreeClick()
    {
        Engine.TimeScale = levelThreeSimulationSpeed;
        this.SimulationSpeedLevelOneButton.SetPressed(true);
        this.SimulationSpeedLevelTwoButton.SetPressed(true);
        this.SimulationSpeedLevelThreeButton.SetPressed(true);
        this.SimulationSpeedLevelFourButton.SetPressed(false);
    }

    private void OnSimulationSpeedLevelFourClick()
    {
        Engine.TimeScale = levelFourSimulationSpeed;
        this.SimulationSpeedLevelOneButton.SetPressed(true);
        this.SimulationSpeedLevelTwoButton.SetPressed(true);
        this.SimulationSpeedLevelThreeButton.SetPressed(true);
        this.SimulationSpeedLevelFourButton.SetPressed(true);
    }

    private void OnSimulationViewClick()
    {
        this.ViewMode = ViewMode.Simulation;
        this.UpdateViewButtonsState();
        this.UpdateViewVisibility();
        this.UpdateFadeoutBackgroundVisibility();
    }

    private void OnAnalysisViewClick()
    {
        this.ViewMode = ViewMode.Analysis;
        this.UpdateViewButtonsState();
        this.UpdateViewVisibility();
        this.UpdateFadeoutBackgroundVisibility();
    }

    private void OnConfigurationViewClick()
    {
        this.ViewMode = ViewMode.Configuration;
        this.UpdateViewButtonsState();
        this.UpdateViewVisibility();
        this.UpdateFadeoutBackgroundVisibility();
    }

    private void OnCenterCameraClick()
    {
        this.Camera.MoveTo(EnvironmentManager.Instance.Environment.Size/2.0f);
    }

    public override void _PhysicsProcess(double delta)
    {
        SimulationManager.Instance.Process(delta);
        this.UpdateTimePassed();
        this.UpdateSystemTime();
    }

    private void UpdateTimePassed()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(SimulationManager.Instance.TimePassed);
        this.SimulationTimeLabel.Text = timeSpan.ToString(@"hh\:mm\:ss");
    }

    private void UpdateSystemTime()
    {
        this.SystemTimeLabel.Text = Time.GetTimeStringFromSystem();
    }

    private void UpdatePauseLabelVisibility()
    {
        this.PauseNode.Visible = SimulationManager.Instance.IsSimulationPaused;
    }

    private void UpdateViewButtonsState()
    {
        this.SimulationViewButton.Disabled = this.ViewMode == ViewMode.Simulation;
        this.SimulationViewButton.UpdateAfterDisabledChange();
        this.AnalysisViewButton.Disabled = this.ViewMode == ViewMode.Analysis;
        this.AnalysisViewButton.UpdateAfterDisabledChange();
        this.ConfigurationViewButton.Disabled = this.ViewMode == ViewMode.Configuration;
        this.ConfigurationViewButton.UpdateAfterDisabledChange();
    }

    private void UpdateViewVisibility()
    {
        this.SimulationViewNode.Visible = this.ViewMode == ViewMode.Simulation || this.ViewMode == ViewMode.Analysis;
        this.AnalysisViewNode.Visible = this.ViewMode == ViewMode.Analysis;
        this.ConfigurationViewNode.Visible = this.ViewMode == ViewMode.Configuration;
    }

    private void UpdateFadeoutBackgroundVisibility()
    {
        this.FadeoutBackground.Visible = this.ViewMode != ViewMode.Simulation || this.SubmenuNode.Visible;
        GD.Print(this.FadeoutBackground.Visible);
    }
}