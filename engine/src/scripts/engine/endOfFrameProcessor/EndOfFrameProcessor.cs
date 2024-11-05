using Godot;

public partial class EndOfFrameProcessor : Node
{
    public override void _Ready()
    {
        NeatPrinter.Start().Print("SCENE LOADING COMPLETE").End();
        if (Reloader.Get().IsReloading)
        {
            Reloader.Get().IsReloading = false;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        EventManager.Get().EmitDelayedEvents();
    }

    public EndOfFrameProcessor()
    {

    }
}