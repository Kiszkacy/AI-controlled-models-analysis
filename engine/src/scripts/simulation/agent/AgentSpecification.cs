
using Godot;

public interface AgentSpecification
{
    public float MaximumSpeed { get; set; } // in px per sec
    public float MaximumAcceleration { get; set; } // in px per sec
    public float MaximumDeceleration { get; set; } // in px per sec
    public float MaximumEnergy { get; set; }
    public float InitialEnergy { get; set; }
    public float MaximumHealth { get; set; }
    public float InitialHealth { get; set; }
    public float MaximumTurnSpeed { get; set; } // in radians per sec
    public float SightAngle { get; set; } // in radians
    public float SightRadius { get; set; } // in px
    public float EnergySurplusForReproduction { get; set; }
    public float MinimumHealthToReproduce { get; set; }

    public float ReproductionEnergyCost { get; set; }
    public float EnergyLossPerSecond { get; set; }
    public float EnergyLossPerSecondPer100UnitsOfMovement { get; set; }
    public float EnergyLossPerSecondRotation { get; set; }

    public float HealthLossPerSecond { get; set; }
    public float HealthRegenPerSecond { get; set; }

    public Color Color { get; set; }
    public float SizeMultiplier { get; set; }
}