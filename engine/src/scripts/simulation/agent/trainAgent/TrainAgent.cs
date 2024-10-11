

using Godot.Collections;

public partial class TrainAgent : Agent
{
    private float thisFrameScore = 0.0f;

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
    }

    private void Act()
    {
        this.Accelerate(this.Action.AccelerateStrength);
        this.Rotate(this.Action.RotateStrength);
    }

    public override void Save(Dictionary data)
    {
        base.Save(data);
        data["thisFrameScore"] = this.thisFrameScore;
    }

    public override void Load(Dictionary data)
    {
        base.Load(data);
        this.thisFrameScore = (float)data["thisFrameScore"];
    }
}