using Godot;

public partial class EndOfFrameProcessor : Node
{
    public override void _Ready()
    {
        GD.Print("EndOfFrameProcessor: Scene loading complete.");
    }

    public override void _PhysicsProcess(double delta)
    {
        EventManager.Get().EmitDelayedEvents();
        this.GetParent().GetNode<Label>("HUD/Position").Text = this.GetParent().GetNode<Node2D>("Camera").GlobalPosition.ToString();
    }
    
    public EndOfFrameProcessor()
    {
		
    }
}