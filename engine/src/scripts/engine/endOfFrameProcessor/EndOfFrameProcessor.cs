using Godot;

public partial class EndOfFrameProcessor : Node
{
    public override void _Ready()
    {
        NeatPrinter.Start().Print("SCENE LOADING COMPLETE").End();
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