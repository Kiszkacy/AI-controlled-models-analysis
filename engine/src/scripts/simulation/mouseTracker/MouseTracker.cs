

using Godot;

public partial class MouseTracker : Node2D
{
    private RayCast2D rayCast;
    private Node2D activeObject;

    public override void _Ready()
    {
        rayCast = GetNode<RayCast2D>("RayCast2D");
        rayCast.Enabled = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left &&
            mouseEvent.IsPressed())
        {
            rayCast.GlobalPosition = GetGlobalMousePosition();
            if (rayCast.IsColliding())
            {
                activeObject = rayCast.GetCollider() as Node2D;
                if (activeObject is Agent agent)
                {
                    DisplayStats(agent.GetStats());
                }
                else if (activeObject is EnvironmentObject environmentObject)
                {
                    DisplayStats(environmentObject.GetStats());
                }
                else
                {
                    activeObject = null;
                    HideStats();
                }
            }
        }
    }

    private void DisplayStats(string stats)
    {

    }

    private void HideStats()
    {

    }
}