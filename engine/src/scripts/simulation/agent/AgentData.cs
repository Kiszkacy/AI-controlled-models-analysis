
using Godot;

public readonly struct AgentData
{
    public int Id { get; }
    public float Score { get; }
    public float Speed { get; }
    public float Energy { get; }
    public float Health { get; }
    public float DistanceToClosestFood { get; }
    public float AngleToClosestFood { get; }

    public AgentData(int id, float score, float speed, float energy, float health, float distanceToClosestFood, float angleToClosestFood)
    {
        this.Id = id;
        this.Score = score;
        this.Speed = speed;
        this.Energy = energy;
        this.Health = health;
        this.DistanceToClosestFood = distanceToClosestFood;
        this.AngleToClosestFood = angleToClosestFood;
    }

    public AgentData Normalize(Agent agent)
    {
        return new AgentData(
            id: this.Id,
            score: this.Score,
            speed: Mathf.Remap(this.Speed, 0.0f, agent.MaximumSpeed, -1, 1),
            energy: Mathf.Remap(this.Energy, 0.0f, agent.MaximumEnergy, -1, 1),
            health: Mathf.Remap(this.Health, 0.0f, agent.MaximumHealth, -1, 1),
            distanceToClosestFood: Mathf.Remap(float.IsNaN(this.DistanceToClosestFood) ? -999 : this.DistanceToClosestFood, 0.0f, agent.SightRadius, -1, 1),
            angleToClosestFood: Mathf.Remap(float.IsNaN(this.AngleToClosestFood) ? -999 : this.AngleToClosestFood, -agent.SightAngle/2.0f, agent.SightAngle/2.0f, -1, 1)
        );
    }

    public override string ToString()
    {
        return $"<id: {this.Id}, score: {this.Score}, speed: {this.Speed}, energy: {this.Energy}, health: {this.Health}, distanceToClosestFood: {this.DistanceToClosestFood}, angleToClosestFood: {this.AngleToClosestFood}>";
    }
}