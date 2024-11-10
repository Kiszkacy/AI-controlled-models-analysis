using System;

using Godot;

public partial class MainMenu : Control
{
    private enum ButtonType
    {
        Continue,
        NewSimulation,
        Simulations,
        Models,
        Settings,
        Exit
    }

    [Export]
    public Button ContinueButton;
    [Export]
    public Button NewSimulationButton;
    [Export]
    public Button SimulationsButton;
    [Export]
    public Button ModelsButton;
    [Export]
    public Button SettingsButton;
    [Export]
    public Button ExitButton;

    [Export]
    public Control HelpBoxParent;
    [Export]
    public RichTextLabel HelpBoxLabel;

    private Timer fadeoutTimer;
    private float fadeoutDuration = 1.0f; // in seconds

    public override void _Ready()
    {
        this.ConnectButtons();
        this.fadeoutTimer = new(this.OnFadeoutTimeout);
    }

    private void ConnectButtons()
    {
        this.ContinueButton.Pressed += this.OnContinueClick;
        this.ContinueButton.MouseEntered += () => this.OnMouseEntered(ButtonType.Continue);
        this.ContinueButton.MouseExited += () => this.OnMouseExited(ButtonType.Continue);

        this.NewSimulationButton.Pressed += this.OnNewSimulationClick;
        this.NewSimulationButton.MouseEntered += () => this.OnMouseEntered(ButtonType.NewSimulation);
        this.NewSimulationButton.MouseExited += () => this.OnMouseExited(ButtonType.NewSimulation);

        this.SimulationsButton.Pressed += this.OnSimulationsClick;
        this.SimulationsButton.MouseEntered += () => this.OnMouseEntered(ButtonType.Simulations);
        this.SimulationsButton.MouseExited += () => this.OnMouseExited(ButtonType.Simulations);

        this.ModelsButton.Pressed += this.OnModelsClick;
        this.ModelsButton.MouseEntered += () => this.OnMouseEntered(ButtonType.Models);
        this.ModelsButton.MouseExited += () => this.OnMouseExited(ButtonType.Models);

        this.SettingsButton.Pressed += this.OnSettingsClick;
        this.SettingsButton.MouseEntered += () => this.OnMouseEntered(ButtonType.Settings);
        this.SettingsButton.MouseExited += () => this.OnMouseExited(ButtonType.Settings);

        this.ExitButton.Pressed += this.OnExitClick;
        this.ExitButton.MouseEntered += () => this.OnMouseEntered(ButtonType.Exit);
        this.ExitButton.MouseExited += () => this.OnMouseExited(ButtonType.Exit);
    }

    public override void _Process(double delta)
    {
        if (this.fadeoutTimer.IsActive)
        {
            this.fadeoutTimer.Process(delta);
            this.HelpBoxParent.Modulate =  new Color(1.0f, 1.0f, 1.0f, (float)(this.fadeoutTimer.Time/this.fadeoutDuration));
        }
    }

    private void OnContinueClick()
    {
        // TODO
    }

    private void OnNewSimulationClick()
    {
        PythonManager.Instance.Start();
        this.GetTree().ChangeSceneToFile("res://src/scenes/simulation/simulation.tscn");
    }

    private void OnSimulationsClick()
    {
        // TODO
    }

    private void OnModelsClick()
    {
        // TODO
    }

    private void OnSettingsClick()
    {
        this.GetTree().ChangeSceneToFile("res://src/scenes/settings.tscn");
    }

    private void OnExitClick()
    {
        this.GetTree().Quit();
    }

    private void OnMouseEntered(ButtonType whichButton)
    {
        this.fadeoutTimer.Stop();
        this.UpdateHelpBox(whichButton);
    }

    private void OnMouseExited(ButtonType whichButton)
    {
        this.fadeoutTimer.Activate(this.fadeoutDuration);
    }

    private void UpdateHelpBox(ButtonType activeButton)
    {
        this.HelpBoxParent.Visible = true;
        this.HelpBoxParent.Modulate = new Color(1.0f, 1.0f, 1.0f);

        switch (activeButton)
        {
            case ButtonType.Continue:
                this.SetHelpBoxText("Load and run your most recent simulation.");
                break;
            case ButtonType.NewSimulation:
                this.SetHelpBoxText("Create new simulation.");
                break;
            case ButtonType.Simulations:
                this.SetHelpBoxText("Browse, delete and edit your created simulations.");
                break;
            case ButtonType.Models:
                this.SetHelpBoxText("Browse, delete and edit your created models or create new one.");
                break;
            case ButtonType.Settings:
                this.SetHelpBoxText("Change application settings.");
                break;
            case ButtonType.Exit:
                this.SetHelpBoxText("Close the application.");
                break;
        }
    }

    private void SetHelpBoxText(string to)
    {
        this.HelpBoxLabel.Text = $"[center]{to}[/center]";
    }

    private void OnFadeoutTimeout()
    {
        this.HelpBoxParent.Visible = false;
    }
}
