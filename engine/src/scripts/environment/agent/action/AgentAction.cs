using Newtonsoft.Json;

public struct AgentAction
{
    public int Id { get; }
    public float AccelerateStrength { get; }
    public float RotateStrength { get; }

    [JsonConstructor]
    public AgentAction([JsonProperty("id")] int id, [JsonProperty("accelerate")] float accelerateStrength, [JsonProperty("rotate")] float rotateStrength)
    {
        this.Id = id;
        this.AccelerateStrength = accelerateStrength;
        this.RotateStrength = rotateStrength;
    }
}