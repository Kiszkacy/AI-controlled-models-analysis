using Godot;

public struct AgentSaveData
{
    public int Id { get; set; }
    public Vector2 Position { get; set; }
    public float MaximumSpeed { get; set; }
    public float MaximumAcceleration { get; set; }
    public float MaximumDeceleration { get; set; }
    public float MaximumEnergy { get; set; }
    public float InitialEnergy { get; set; }
    public float MaximumHealth { get; set; }
    public float InitialHealth { get; set; }
    public float MaximumTurnSpeed { get; set; }
    public float SightAngle { get; set; }
    public float SightRadius { get; set; }
    public float Energy { get; set; }
    public float Health { get; set; }
    public float CurrentRotation { get; set; }
    public float CurrentAcceleration { get; set; }
    public Vector2 Direction { get; set; }
    public Vector2 Velocity { get; set; }
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
    public float? ThisFrameScore { get; set; }

    public AgentSaveData(Vector2 position, float maximumSpeed, float maximumAcceleration, float maximumDeceleration,
        float maximumEnergy, float initialEnergy, float maximumHealth, float initialHealth, float maximumTurnSpeed,
        float sightAngle, float sightRadius, float energy, float health, float currentRotation, float currentAcceleration,
        Vector2 direction, Vector2 velocity, int id, float energySurplusForReproduction, float minimumHealthToReproduce,
        float reproductionEnergyCost, float energyLossPerSecond, float energyLossPerSecondPer100UnitsOfMovement, float energyLossPerSecondRotation,
        float healthLossPerSecond, float healthRegenPerSecond, Color color, float sizeMultiplier, float? thisFrameScore = null)
    {
        this.Position = position;
        this.MaximumSpeed = maximumSpeed;
        this.MaximumAcceleration = maximumAcceleration;
        this.MaximumDeceleration = maximumDeceleration;
        this.MaximumEnergy = maximumEnergy;
        this.InitialEnergy = initialEnergy;
        this.MaximumHealth = maximumHealth;
        this.InitialHealth = initialHealth;
        this.MaximumTurnSpeed = maximumTurnSpeed;
        this.SightAngle = sightAngle;
        this.SightRadius = sightRadius;
        this.Energy = energy;
        this.Health = health;
        this.CurrentRotation = currentRotation;
        this.CurrentAcceleration = currentAcceleration;
        this.Direction = direction;
        this.Velocity = velocity;
        this.EnergySurplusForReproduction = energySurplusForReproduction;
        this.MinimumHealthToReproduce = minimumHealthToReproduce;
        this.ReproductionEnergyCost = reproductionEnergyCost;
        this.EnergyLossPerSecond = energyLossPerSecond;
        this.EnergyLossPerSecondPer100UnitsOfMovement = energyLossPerSecondPer100UnitsOfMovement;
        this.EnergyLossPerSecondRotation = energyLossPerSecondRotation;
        this.HealthLossPerSecond = healthLossPerSecond;
        this.HealthRegenPerSecond = healthRegenPerSecond;
        this.Color = color;
        this.SizeMultiplier = sizeMultiplier;
        this.Id = id;
        this.ThisFrameScore = thisFrameScore;
    }
}