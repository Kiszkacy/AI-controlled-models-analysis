using Godot;
using Godot.Collections;

public class AgentSaveData
{
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
    public int Id { get; set; }

    public Dictionary ToDictionary()
    {
        return new Dictionary
        {
            { "Position", this.Position },
            { "MaximumSpeed", this.MaximumSpeed },
            { "MaximumAcceleration", this.MaximumAcceleration },
            { "MaximumDeceleration", this.MaximumDeceleration },
            { "MaximumEnergy", this.MaximumEnergy },
            { "InitialEnergy", this.InitialEnergy },
            { "MaximumHealth", this.MaximumHealth },
            { "InitialHealth", this.InitialHealth },
            { "MaximumTurnSpeed", this.MaximumTurnSpeed },
            { "SightAngle", this.SightAngle },
            { "SightRadius", this.SightRadius },
            { "Energy", this.Energy },
            { "Health", this.Health },
            { "CurrentRotation", this.CurrentRotation },
            { "CurrentAcceleration", this.CurrentAcceleration },
            { "Direction", this.Direction },
            { "Velocity", this.Velocity },
            { "Id", this.Id },
        };
    }
}