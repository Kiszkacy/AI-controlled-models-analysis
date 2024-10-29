

using Godot;

public partial class ObjectTracker : Node2D
{
    private Node2D activeObject;
    private Camera camera;

    public override void _Ready()
    {
        camera = GetNode<Camera>("/root/Root/Camera");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
        {
            Vector2 mousePos = GetGlobalMousePosition();
            var spaceState = GetWorld2D().DirectSpaceState;
            var query = new PhysicsPointQueryParameters2D();
            query.SetPosition(mousePos);
            query.SetCollisionMask(1);
            query.SetCollideWithBodies(true);
            query.SetCollideWithAreas(false);
            var result = spaceState.IntersectPoint(query);

            GD.Print($"Click at position: {mousePos}");
            GD.Print($"Found {result.Count} colliding objects");

            if (result.Count > 0)
            {
                var collider = result[0]["collider"].As<Node2D>();
                GD.Print($"Clicked on: {collider.Name}");
                SetTracking(collider);
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
    private void SetTracking(Node2D obj)
    {
        activeObject = obj;
        if (activeObject is Agent agent)
        {
            DisplayStats(agent.GetStats());
        }
        else if (activeObject is EnvironmentObject envObject)
        {
            DisplayStats(envObject.GetStats());
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