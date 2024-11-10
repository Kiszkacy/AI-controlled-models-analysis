

using Godot;

public partial class ObjectTracker : Node2D
{
    [Export]
    private Camera camera;
    [Export]
    private Label TargetLabel;

    [System.Obsolete]
    private BiomeMap biomeMap;
    private Node2D activeObject;
    private string lastStats;

    [System.Obsolete]
    public override void _Ready()
    {
        this.biomeMap = GetNode<BiomeMap>("/root/Root/Environment/BiomeMap");
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
                if (collider is Trackable)
                {
                    this.SetTracking(collider);
                }
                else
                {
                    var parent = (Node2D)collider.GetParent();
                    this.SetTracking(parent);
                }
            }
            else
            {
                this.SetTracking(null);
            }
        }
        else if (@event.IsActionPressed("focus.on.active.object"))
        {
            if (activeObject != null)
            {
                this.camera.Follow(activeObject);
            }
        }
    }

    [System.Obsolete]
    public override void _Process(double delta)
    {
        Vector2 mousePos = this.GetGlobalMousePosition();
        BiomeType currentBiome = this.biomeMap.GetBiomeAtGlobalPosition(mousePos);
        this.DisplayBiomeType(currentBiome);
        this.CheckIfActiveObjectExists();
    }

    private void SetTracking(Node2D obj)
    {
        this.activeObject = obj;
        EventManager.Instance.RegisterEvent(new NodeEvent(this.activeObject), EventChannel.ObjectTracker);
    }

    private void DisplayBiomeType(BiomeType biome)
    {
        // TODO add to UI
        NeatPrinter.Start().Print(biome.ToString()).End();
    }

    private void CheckIfActiveObjectExists()
    {
        if (this.activeObject != null && !IsInstanceValid(this.activeObject)) // if was deleted via godot by queueFree e.g. agent died
        {
            this.activeObject = null;
            EventManager.Instance.RegisterEvent(new NodeEvent(this.activeObject), EventChannel.ObjectTracker);
        }
    }
}