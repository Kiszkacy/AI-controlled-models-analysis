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
    // [Export]
    // private EnvironmentConfiguration AgentConfiguration;
    // [Export]
    // private EnvironmentConfiguration TrainingConfiguration;

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
    
    private void OnContinueClick()
    {
        switch (this.stage)
        {
            case ConfigurationStage.Environment:
                this.environmentGenerator = this.EnvironmentConfiguration.GetEnvironmentGenerator();
                break;
            case ConfigurationStage.Agents:
                // this.EnvironmentConfiguration.Visible = false;
                break;
            case ConfigurationStage.Training:
                // this.EnvironmentConfiguration.Visible = false;
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
