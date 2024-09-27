using Godot;

public partial class Camera : Camera2D
{
    [Export(PropertyHint.Range, "100,1000,10,or_greater")]
    public float MoveSpeed { get; set; } = 300.0f; // in px/sec

    [Export(PropertyHint.Range, "0.1,2.0,0.1,or_greater")]
    public float ZoomSpeed { get; set; } = 1.0f; // per sec

    [Export(PropertyHint.Range, "1.0,5.0,0.1,or_greater")]
    public float MaxZoom { get; set; } = 3.0f;

    [Export(PropertyHint.Range, "0.1,1.0,0.1")]
    public float MinZoom { get; set; } = 0.3f;

    [Export(PropertyHint.Range, "0.1,1.0,0.01")]
    public float DragSmoothness { get; set; } = 0.15f;

    private Vector2 moveDirection = Vector2.Zero;
    private bool zoomingIn = false;
    private bool zoomingOut = false;
    private bool zoomingInByMouse = false;
    private bool zoomingOutByMouse = false;
    private bool isDragging = false;
    private Vector2 dragTarget = Vector2.Zero;
    private Vector2 mouseDragStart = Vector2.Zero;

    public override void _Input(InputEvent @event)
    {
        HandleKeyboardInput(@event);
        HandleMouseInput(@event);
    }

    private void HandleKeyboardInput(InputEvent @event)
    {

        if (@event.IsActionPressed("move.camera.up"))
        {
            this.moveDirection += Vector2.Up;
        }
        if (@event.IsActionPressed("move.camera.right"))
        {
            this.moveDirection += Vector2.Right;
        }
        if (@event.IsActionPressed("move.camera.down"))
        {
            this.moveDirection += Vector2.Down;
        }
        if (@event.IsActionPressed("move.camera.left"))
        {
            this.moveDirection += Vector2.Left;
        }
        if (@event.IsActionPressed("zoom.camera.in"))
        {
            this.zoomingIn = true;
        }
        if (@event.IsActionPressed("zoom.camera.out"))
        {
            this.zoomingOut = true;
        }

        if (@event.IsActionReleased("move.camera.up"))
        {
            this.moveDirection -= Vector2.Up;
        }
        if (@event.IsActionReleased("move.camera.right"))
        {
            this.moveDirection -= Vector2.Right;
        }
        if (@event.IsActionReleased("move.camera.down"))
        {
            this.moveDirection -= Vector2.Down;
        }
        if (@event.IsActionReleased("move.camera.left"))
        {
            this.moveDirection -= Vector2.Left;
        }
        if (@event.IsActionReleased("zoom.camera.in"))
        {
            this.zoomingIn = false;
        }
        if (@event.IsActionReleased("zoom.camera.out"))
        {
            this.zoomingOut = false;
        }
    }

    private void HandleMouseInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                this.isDragging = true;
                this.dragTarget = this.GlobalPosition;
                this.mouseDragStart = GetGlobalMousePosition();
            }

            else if (!mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                this.isDragging = false;
            }

            if (mouseEvent.DoubleClick && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                this.GlobalPosition = GetGlobalMousePosition();
                this.isDragging = false;
            }
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                this.zoomingInByMouse = true;
            }
            if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                this.zoomingOutByMouse = true;
            }
        }
        else if (@event is InputEventMouseMotion motionEvent && isDragging)
        {
            Vector2 currentMouse = GetGlobalMousePosition();
            Vector2 drag = this.mouseDragStart - currentMouse;
            this.dragTarget += drag;
            this.mouseDragStart = currentMouse;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        this.UpdatePosition(delta);
        this.UpdateZoom(delta);
        SmoothDragMovement(delta);
    }

    private void UpdatePosition(double delta)
    {
        this.GlobalPosition += this.moveDirection * this.MoveSpeed * (float)delta;
    }

    private void UpdateZoom(double delta)
    {
        Vector2 zoomRatio = Vector2.Zero;
        if (this.zoomingIn || this.zoomingInByMouse) zoomRatio += new Vector2(this.ZoomSpeed, this.ZoomSpeed);
        if (this.zoomingOut || this.zoomingOutByMouse) zoomRatio -= new Vector2(this.ZoomSpeed, this.ZoomSpeed);
        this.Zoom += zoomRatio * (float)delta;
        this.Zoom = this.Zoom.Clamp(new Vector2(this.MinZoom, this.MinZoom), new Vector2(this.MaxZoom, this.MaxZoom));

        this.zoomingInByMouse = false;
        this.zoomingOutByMouse = false;
    }
    private void SmoothDragMovement(double delta)
    {
        if (isDragging && GlobalPosition.DistanceTo(dragTarget) > 0.1f)
        {
            GlobalPosition = GlobalPosition.Lerp(dragTarget, DragSmoothness);
        }
    }
}