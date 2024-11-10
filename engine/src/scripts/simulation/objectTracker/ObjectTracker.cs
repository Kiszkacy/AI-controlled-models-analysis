

using Godot;

public partial class ObjectTracker : Node2D
{
    private Node2D activeObject;
    private Camera camera;
    [System.Obsolete]
    private BiomeMap biomeMap;
    private string lastStats;

    [System.Obsolete]
    public override void _Ready()
    {
        camera = GetNode<Camera>("/root/Root/Camera");
        biomeMap = GetNode<BiomeMap>("/root/Root/Environment/BiomeMap");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right })
        {
            Vector2 mousePos = GetGlobalMousePosition();
            var spaceState = GetWorld2D().DirectSpaceState;
            var query = new PhysicsPointQueryParameters2D();
            query.SetPosition(mousePos);
            query.SetCollideWithBodies(true);
            query.SetCollideWithAreas(true);
            var result = spaceState.IntersectPoint(query);

            if (result.Count > 0)
            {
                var collider = result[0]["collider"].As<Node2D>();
                if (collider is Agent || collider is EnvironmentObject)
                {
                    SetTracking(collider);
                }
                else
                {
                    var parent = (Node2D)collider.GetParent();
                    SetTracking(parent);
                }
            }
        }
        else if (@event.IsActionPressed("focus.on.active.object"))
        {
            if (activeObject != null)
            {
                camera.Follow(activeObject);
            }
        }
    }

    [System.Obsolete]
    public override void _Process(double delta)
    {
        Vector2 mousePos = GetGlobalMousePosition();
        BiomeType currentBiome = biomeMap.GetBiomeAtGlobalPosition(mousePos);
        DisplayBiomeType(currentBiome);

        if (activeObject != null)
        {
            string currentStats = "";
            if (activeObject is Agent agent)
            {
                currentStats = agent.GetStats();
            }
            else if (activeObject is EnvironmentObject envObject)
            {
                currentStats = envObject.GetStats();
            }

            if (currentStats != lastStats)
            {
                HideStats();
                DisplayStats(currentStats);
                lastStats = currentStats;
            }
        }
    }

    private void SetTracking(Node2D obj)
    {
        activeObject = obj;
        string stats = "";
        if (activeObject is Agent agent)
        {
            stats = agent.GetStats();
        }
        else if (activeObject is EnvironmentObject envObject)
        {
            stats = envObject.GetStats();
        }

        lastStats = stats;
        DisplayStats(stats);
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

    private void DisplayBiomeType(BiomeType biome)
    {

    }
}