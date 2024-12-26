
using Godot;

public class AgentTemplate : AgentSpecification
{
    private readonly PackedScene packedTrainAgent = ResourceLoader.Load<PackedScene>("res://src/scenes/simulation/agent/trainAgent.tscn");

    public float MaximumSpeed { get; set; } = 200.0f; // in px per sec
    public float MaximumAcceleration { get; set; } = 100.0f; // in px per sec
    public float MaximumDeceleration { get; set; } = 100.0f; // in px per sec
    public float MaximumEnergy { get; set; } = 200.0f;
    public float InitialEnergy { get; set; } = 100.0f;
    public float MaximumHealth { get; set; } = 100.0f;
    public float InitialHealth { get; set; } = 100.0f;
    public float MaximumTurnSpeed { get; set; } = Mathf.Pi / 2.0f; // in radians per sec
    public float SightAngle { get; set; } = Mathf.Pi / 4.0f; // in radians
    public float SightRadius { get; set; } = 256.0f; // in px
    public float EnergySurplusForReproduction { get; set; } = 30.0f;
    public float MinimumHealthToReproduce { get; set; } = 50.0f;

    public float ReproductionEnergyCost { get; set; } = 50.0f;
    public float EnergyLossPerSecond { get; set; } = 2.0f;
    public float EnergyLossPerSecondPer100UnitsOfMovement { get; set; } = 0.2f;
    public float EnergyLossPerSecondRotation { get; set; } = 0.2f;

    public float HealthLossPerSecond { get; set; } = 5.0f;
    public float HealthRegenPerSecond { get; set; } = 1.0f;

    public Color Color { get; set; } = Color.FromHsv(1.0f, 0.0f, 1.0f);
    public float SizeMultiplier { get; set; } = 1.0f;

    public void Instantiate(Node node)
    {
        TrainAgent agent = packedTrainAgent.Instantiate<TrainAgent>();
    }
}