
using Godot;

public partial class CameraBorder : Control
{
    [Export] 
    public Camera Camera;

    private readonly int sizeOfTheMinimap = 256; // TODO its hardcoded ! in px

    public override void _Process(double delta)
    {
        this.UpdateSize();
        this.UpdatePosition();
    }

    private void UpdateSize()
    {
        // TODO this currently assumes that the screen size is 1920 x 1080 wont work properly on other resolutions
        Vector2 screenResolution = new(1920, 1080);
        GD.Print(this.Camera.Zoom.Inverse());
        Vector2 environmentSize = EnvironmentManager.Instance.Environment.Size;
        Vector2 visibleEnvironmentSize = this.Camera.Zoom.Inverse() * screenResolution;
        Vector2 calculatedScale = visibleEnvironmentSize / environmentSize;
        this.Size = calculatedScale * sizeOfTheMinimap;
    }
    
    private void UpdatePosition()
    {
        this.Position = (this.Camera.GlobalPosition / EnvironmentManager.Instance.Environment.Size) * sizeOfTheMinimap - this.Size/2.0f;
    }
}