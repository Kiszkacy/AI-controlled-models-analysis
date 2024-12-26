

using Godot;

public partial class TrainAgent : Agent
{
    private float thisFrameScore = 0.0f;
    private float currentWaterPenalty = 0.0f;
    private Vector2 checkDistance;

    public float Score
    {
        get
        {
            float score = this.thisFrameScore;
            this.thisFrameScore = 0.0f;
            return score;
        }
    }

    public AgentData Data => new(this.Id, this.Score, this.Speed, this.energy, this.health, this.DistanceToClosestFood, this.AngleToClosestFood);

    public AgentData NormalizedData => this.Data.Normalize(this);

    public AgentAction Action;

    protected override void Eat(Food food)
    {
        base.Eat(food);
        this.thisFrameScore += Config.Get().Environment.Score.FoodEaten;
    }

    public override void _PhysicsProcess(double delta)
    {
        this.Act();
        base._PhysicsProcess(delta);
        if (Engine.GetPhysicsFrames() % (ulong)Config.Get().Environment.AgentWaterPenaltyUpdateEveryNthFrame == 0)
        {
            this.UpdateWaterPenalty();
        }
        this.UpdateScores(delta);
    }

    public override void _Ready()
    {
        base._Ready();
        this.checkDistance = EnvironmentManager.Get().Environment.TemplateData.GenerationSettings.TerrainChunkSize;
    }

    private void Act()
    {
        this.Accelerate(this.Action.AccelerateStrength);
        this.Rotate(this.Action.RotateStrength);
    }

    private void UpdateScores(double delta)
    {
        float energyScore = Config.Get().Environment.Score.EnergyMax * (this.energy / this.MaximumEnergy);
        float healthScore = Config.Get().Environment.Score.HealthMax * (this.health / this.MaximumHealth);
        this.thisFrameScore += (energyScore + healthScore - this.currentWaterPenalty) * (float)delta;
    }

    private void UpdateWaterPenalty()
    {
        Vector2[] checkPositions = {
            this.GlobalPosition + new Vector2(0, -checkDistance.Y), // Up
            this.GlobalPosition + new Vector2(0, checkDistance.Y),  // Down
            this.GlobalPosition + new Vector2(-checkDistance.X, 0), // Left
            this.GlobalPosition + new Vector2(checkDistance.X, 0),  // Right
            this.GlobalPosition + new Vector2(-checkDistance.X, -checkDistance.Y), // Top-left (diagonal)
            this.GlobalPosition + new Vector2(checkDistance.X, -checkDistance.Y),  // Top-right (diagonal)
            this.GlobalPosition + new Vector2(-checkDistance.X, checkDistance.Y),  // Bottom-left (diagonal)
            this.GlobalPosition + new Vector2(checkDistance.X, checkDistance.Y)   // Bottom-right (diagonal)
        };

        foreach (Vector2 checkPosition in checkPositions)
        {
            BiomeType biomeType = EnvironmentManager.Get().Environment.GetBiomeAt(checkPosition);
            if (biomeType == BiomeType.Ocean)
            {
                this.currentWaterPenalty = Config.Get().Environment.Score.WaterPenalty;
                return;
            }
        }

        this.currentWaterPenalty = 0.0f;
    }

    protected override bool Reproduce()
    {
        if (base.Reproduce())
        {
            this.thisFrameScore += Config.Get().Environment.Score.Reproduction;
            return true;
        }
        return false;
    }

    public override AgentSaveData Save()
    {
        var data = base.Save();
        data.ThisFrameScore = this.thisFrameScore;
        return data;
    }

    public override void Load(AgentSaveData data)
    {
        base.Load(data);
        if (data.ThisFrameScore.HasValue)
        {
            this.thisFrameScore = data.ThisFrameScore.Value;
        }
    }
}
