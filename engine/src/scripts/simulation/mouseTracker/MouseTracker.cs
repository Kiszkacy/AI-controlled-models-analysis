

using Godot;

public partial class MouseTracker : Node2D
{
    private Area2D clickArea;
    private Node2D activeObject;
    private Camera camera;
    private CollisionShape2D clickShape;
    private CircleShape2D circleShape;
    private const float BaseRadius = 25.0f;

    public override void _Ready()
    {
        clickArea = new Area2D();
        clickShape = new CollisionShape2D();
        circleShape = new CircleShape2D();
        circleShape.Radius = BaseRadius;
        clickShape.Shape = circleShape;
        clickArea.AddChild(clickShape);
        AddChild(clickArea);
        clickArea.CollisionMask = 0xFFFFFFFF;
        clickArea.Monitorable = false;
        clickArea.Monitoring = true;
        camera = GetNode<Camera>("/root/Root/Camera");
    }

    public override void _Process(double delta)
    {
        if (camera != null)
        {
            float zoomLevel = camera.Zoom.X;
            circleShape.Radius = BaseRadius * zoomLevel;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left &&
            mouseEvent.IsPressed())
        {
            GD.Print("Circle radius = " + circleShape.Radius);
            clickArea.GlobalPosition = GetGlobalMousePosition();
            var overlappingBodies = clickArea.GetOverlappingBodies();
            if (overlappingBodies.Count > 0)
            {
                GD.Print("Area is colliding.");
                Node2D clickedObject = overlappingBodies[0];
                if (clickedObject != null)
                {
                    activeObject = clickedObject;
                    if (activeObject is Agent agent)
                    {
                        DisplayStats(agent.GetStats());
                    }
                    else if (activeObject is EnvironmentObject envObject)
                    {
                        DisplayStats(envObject.GetStats());
                    }
                }
            }
            else
            {
                activeObject = null;
                HideStats();
                camera.StopFollowing();
            }
        }
        else if (@event.IsActionPressed("ui_home"))
        {
            if (activeObject != null)
            {
                camera.Follow(activeObject);
            }
        }
    }

    private void DisplayStats(string stats)
    {
        NeatPrinter.Start()
        .Print(stats)
        .End();
    }

    private void HideStats()
    {

    }
}