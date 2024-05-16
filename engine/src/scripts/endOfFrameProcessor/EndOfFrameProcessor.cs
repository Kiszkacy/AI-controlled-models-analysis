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
    }
    
    public EndOfFrameProcessor()
    {
		
    }
}