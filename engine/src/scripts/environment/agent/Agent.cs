using Godot;

public partial class Agent : CharacterBody2D
{
    [Export]
    public float MaximumSpeed { get; set; } = 200.0f; // in px per sec

    [Export]
    public float MaximumAcceleration { get; set; } = 100.0f; // in px per sec

    [Export]
    public float MaximumDeceleration { get; set; } = 100.0f; // in px per sec

    [Export]
    public float MaximumEnergy { get; set; } = 200.0f;

    [Export]
    public float InitialEnergy { get; set; } = 100.0f;

    [Export]
    public float MaximumHealth { get; set; } = 100.0f;

    [Export]
    public float InitialHealth { get; set; } = 100.0f;

    [Export]
    public float MaximumTurnSpeed { get; set; } = Mathf.Pi / 2.0f; // in radians per sec

    [Export]
    public float SightAngle { get; set; } = Mathf.Pi / 4.0f; // in radians

    [Export]
    public float SightRadius { get; set; } = 256.0f; // in px

    protected float energy;
    protected float health;
    protected float currentRotation = 0.0f;
    protected float currentAcceleration = 0.0f;

    protected Sprite2D sprite;

    public Vector2 Direction { get; set; } = Vector2.Right;
    public float DirectionAngle => this.Direction.Angle();
    public float Speed => this.Velocity.Length();

    protected Food closestFood = null;
    public float DistanceToClosestFood => this.closestFood?.GlobalPosition.DistanceTo(this.GlobalPosition) ?? float.NaN; // in radians
    public float AngleToClosestFood => this.closestFood != null ? this.Direction.AngleTo(this.closestFood.GlobalPosition - this.GlobalPosition) : float.NaN; // in radians

    private int id = -1;

    public int Id
    {
        set
        {
            if (this.id != -1) return;
            this.id = value;
        }
        get => this.id;
    }

    protected void UpdateEnergy(double delta)
    {
        float energyLoss = Config.Instance.Environment.EnergyBaseLossPerSecond
                           + Config.Instance.Environment.EnergyLossPerSecondPer100UnitsOfMovement * (this.Velocity.Length()/100.0f)
                           + Config.Instance.Environment.EnergyLossPerSecondTurn * Mathf.Abs(this.currentRotation);
        this.energy -= energyLoss * (float)delta;
    }

    protected void UpdateHealth(double delta)
    {
        if (this.energy <= 0.0f) // TODO proper float comparison
        {
            this.health -= Config.Instance.Environment.HealthLossPerSecond * (float)delta;
        }
        else
        {
            this.health += Config.Instance.Environment.HealthRegenPerSecond * (float)delta;
        }

        this.health = Mathf.Clamp(this.health, 0.0f, this.MaximumHealth);
        if (this.health <= 0.0f) // TODO proper float comparison
        {
            this.Die();
        }
    }

    protected void Die()
    {
        EntityManager.Get().RemoveAgent(this);
        this.QueueFree();
    }

    protected void SightProcess()
    {
        this.closestFood = null;

        foreach (Food food in EntityManager.Get().Food)
        {
            float distanceToFood = food.GlobalPosition.DistanceTo(this.GlobalPosition);
            if (distanceToFood >= this.SightRadius)
            {
                continue;
            }

            if (this.closestFood != null && this.closestFood.GlobalPosition.DistanceTo(this.GlobalPosition) <= distanceToFood)
            {
                continue;
            }

            Vector2 directionToFood = (food.GlobalPosition - this.GlobalPosition).Normalized();
            float angleToFoodInRadians = this.Direction.AngleTo(directionToFood);
            if (Mathf.Abs(angleToFoodInRadians) < this.SightAngle / 2.0f)
            {
                this.closestFood = food;
            }
        }
    }

    protected new void Rotate(float strength) // input of range <-1, 1>
    {
        strength = Mathf.Clamp(strength, -1, 1);
        this.currentRotation = Mathf.Remap(strength, -1, 1, -this.MaximumTurnSpeed, this.MaximumTurnSpeed);
    }

    protected void Accelerate(float strength) // input of range <-1, 1>
    {
        strength = Mathf.Clamp(strength, -1, 1);
        this.currentAcceleration = Mathf.Remap(strength, -1, 1, -this.MaximumDeceleration, this.MaximumAcceleration);
    }

    protected virtual void OnMouthBodyEntered(Node2D body)
    {
        if (body is not Food food) return;
        this.Eat(food);
    }
    protected virtual void Eat(Food food)
    {
        float nutrition = food.Eat();
        this.energy = Mathf.Clamp(this.energy + nutrition, 0.0f, this.MaximumEnergy);
    }

    protected void UpdateColor()
    {
        this.sprite.Modulate = new Color(this.energy/this.MaximumEnergy, 1.0f, 1.0f - this.energy/this.MaximumEnergy);
    }

    protected void MovementProcess(double delta)
    {
        this.Direction = this.Direction.Rotated(this.currentRotation * (float)delta);
        this.GlobalRotation = this.DirectionAngle;

        if (this.currentAcceleration >= 0.0f)
        {
            this.Velocity = this.Direction * Mathf.Clamp(this.Speed + this.currentAcceleration * (float)delta, 0.0f, this.MaximumSpeed);
        }
        else
        {
            this.Velocity = this.Direction * Mathf.Clamp(this.Speed + this.currentAcceleration * (float)delta, 0.0f, this.MaximumSpeed);
        }


        this.MoveAndCollide(this.Velocity * (float)delta);
    }

    public override void _Ready()
    {
        this.sprite = this.GetNode<Sprite2D>("Sprite");

        this.energy = this.InitialEnergy;
        this.health = this.InitialHealth;

        Area2D mouth = this.GetNode<Area2D>("Mouth");
        mouth.AreaEntered += this.OnMouthBodyEntered;
    }

    public override void _Process(double delta)
    {

    }

    public override void _PhysicsProcess(double delta)
    {
        this.UpdateEnergy(delta);
        this.UpdateHealth(delta);
        this.MovementProcess(delta);
        this.SightProcess();
        this.UpdateColor();
    }
}