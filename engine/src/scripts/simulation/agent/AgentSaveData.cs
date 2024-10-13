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
    public float? ThisFrameScore { get; set; }

    public AgentSaveData(Vector2 position, float maximumSpeed, float maximumAcceleration, float maximumDeceleration,
        float maximumEnergy, float initialEnergy, float maximumHealth, float initialHealth, float maximumTurnSpeed,
        float sightAngle, float sightRadius, float energy, float health, float currentRotation, float currentAcceleration,
        Vector2 direction, Vector2 velocity, int id, float? thisFrameScore = null)
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
        this.Id = id;
        this.ThisFrameScore = thisFrameScore;
    }
}