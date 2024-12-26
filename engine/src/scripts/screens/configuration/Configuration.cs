using Godot;


public enum ConfigurationStage
{
    Environment,
    Agents,
    Training,
}
public partial class Configuration : Control
{
    [Export]
    private ButtonHandler ContinueButton;
    [Export]
    private ButtonHandler ReturnButton;
    
    [Export]
    private EnvironmentConfiguration EnvironmentConfiguration;
    [Export]
    private AgentsConfiguration AgentsConfiguration;
    [Export]
    private TrainingConfiguration TrainingConfiguration;
    
    [Export]
    private Label ProgressBar;
    [Export]
    private Label StepLabel;
    [Export]
    private Label StepCountLabel;

    private const string EnvironmentStepLabel = "Configuring environment";
    private const string AgentsStepLabel = "Configuring agents";
    private const string TrainingStepLabel = "Configuring training parameters";
    
    private ConfigurationStage stage = ConfigurationStage.Environment;
    private EnvironmentGenerator environmentGenerator = null;
    
    public override void _Ready()
    {
        this.ConnectEvents();
    }

    private void ConnectEvents()
    {
        this.ContinueButton.Pressed += this.OnContinueClick;
        this.ReturnButton.Pressed += this.OnReturnClick;
    }

    private void OnReturnClick()
    {
        this.stage -= 1;
        switch (this.stage)
        {
            case ConfigurationStage.Environment:
                this.EnvironmentConfiguration.Visible = true;
                this.AgentsConfiguration.Visible = false;
                this.TrainingConfiguration.Visible = false;
                this.StepCountLabel.Text = "Step 1/3";
                this.StepLabel.Text = EnvironmentStepLabel;
                this.ProgressBar.Size = new Vector2(64.0f, this.ProgressBar.Size.Y);
                break;
            case ConfigurationStage.Agents:
                this.EnvironmentConfiguration.Visible = false;
                this.AgentsConfiguration.Visible = true;
                this.TrainingConfiguration.Visible = false;
                this.StepCountLabel.Text = "Step 2/3";
                this.StepLabel.Text = AgentsStepLabel;
                this.ProgressBar.Size = new Vector2(128.0f, this.ProgressBar.Size.Y);
                break;
            case ConfigurationStage.Training:
                break;
        }
        
        this.ReturnButton.Disabled = this.stage == 0;
        this.ReturnButton.UpdateAfterDisabledChange();
    }
    
    private void OnContinueClick()
    {
        switch (this.stage)
        {
            case ConfigurationStage.Environment:
                this.environmentGenerator = this.EnvironmentConfiguration.GetEnvironmentGenerator();
                this.EnvironmentConfiguration.Visible = false;
                this.AgentsConfiguration.Visible = true;
                this.TrainingConfiguration.Visible = false;
                this.StepCountLabel.Text = "Step 2/3";
                this.StepLabel.Text = AgentsStepLabel;
                this.ProgressBar.Size = new Vector2(128.0f, this.ProgressBar.Size.Y);
                break;
            case ConfigurationStage.Agents:
                this.EnvironmentConfiguration.Visible = false;
                this.AgentsConfiguration.Visible = false;
                this.TrainingConfiguration.Visible = true;
                this.StepCountLabel.Text = "Step 3/3";
                this.StepLabel.Text = TrainingStepLabel;
                this.ProgressBar.Size = new Vector2(192.0f, this.ProgressBar.Size.Y);
                break;
            case ConfigurationStage.Training:
                break;
        }

        if (this.stage == ConfigurationStage.Training) // last stage
        {
            Node root = GetTree().Root;
            NeatPrinter.Start()
                .Print("  | LOADING SELECTED SIMULATION")
                .End();
            Reloader.Get().EnvironmentGeneratorToUseWhenEnteringSimulation = this.environmentGenerator;
            root.GetTree().ChangeSceneToFile("res://src/scenes/simulation/simulation.tscn");
        }
        
        this.stage += 1;
        switch (this.stage)
        {
            case ConfigurationStage.Environment:
                this.EnvironmentConfiguration.Visible = true;
                break;
            case ConfigurationStage.Agents:
                this.EnvironmentConfiguration.Visible = false;
                break;
            case ConfigurationStage.Training:
                this.EnvironmentConfiguration.Visible = false;
                break;
        }

        this.ReturnButton.Disabled = this.stage == 0;
        this.ReturnButton.UpdateAfterDisabledChange();
    }
}
