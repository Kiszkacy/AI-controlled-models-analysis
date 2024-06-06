
public struct AgentAction
{
    public int Id { get; }
    public float AccelerateStrength { get; }
    public float RotateStrength { get; }

    public AgentAction(int id, float accelerateStrength, float rotateStrength)
    {
        this.Id = id;
        this.AccelerateStrength = accelerateStrength;
        this.RotateStrength = rotateStrength;
    }
}