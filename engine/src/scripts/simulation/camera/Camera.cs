using Godot;

public partial class Camera : Camera2D
{
    [Export(PropertyHint.Range, "100,1000,10,or_greater")]
    public float MoveSpeed { get; set; } = 300.0f; // in px/sec

    [Export(PropertyHint.Range, "0.1,2.0,0.1,or_greater")]
    public float ZoomSpeedKeys { get; set; } = 1.0f; // per sec

    [Export(PropertyHint.Range, "0.5,5.0,0.1,or_greater")]
    public float ZoomSpeedMouse { get; set; } = 2.5f; // per sec

    [Export(PropertyHint.Range, "1.0,5.0,0.1,or_greater")]
    public float MaxZoom { get; set; } = 3.0f;

    [Export(PropertyHint.Range, "0.1,1.0,0.1")]
    public float MinZoom { get; set; } = 0.3f;

    [Export(PropertyHint.Range, "0.1,1.0,0.01")]
    public float DragSmoothness { get; set; } = 0.15f;

    [Export(PropertyHint.Range, "0.1,1.0,0.01")]
    public float DoubleClickSmoothness { get; set; } = 0.15f;

    [Export(PropertyHint.Range, "10,100,1")]
    public float EdgeMoveMargin { get; set; } = 50f;

    [Export(PropertyHint.Range, "0.5,10.0,0.1")]
    public float EdgeMoveSpeedQuantifier { get; set; } = 3f;


    private Vector2 moveDirection = Vector2.Zero;
    private Vector2 edgeMoveDirection = Vector2.Zero;
    private bool zoomingIn = false;
    private bool zoomingOut = false;
    private bool zoomingInByMouse = false;
    private bool zoomingOutByMouse = false;
    private bool isDragging = false;
    private Vector2 dragTarget = Vector2.Zero;
    private Vector2 mouseDragStart = Vector2.Zero;
    private bool isDoubleClicked = false;
    private Vector2 doubleClickTarget = Vector2.Zero;

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
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.IsPressed())
                {
                    this.StartDragging();
                }
                else
                {
                    this.StopDragging();
                }

                if (mouseEvent.DoubleClick)
                {
                    this.CenterOnMousePosition();
                }
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                this.zoomingInByMouse = true;
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                this.zoomingOutByMouse = true;
            }
        }
        else if (@event is InputEventMouseMotion motionEvent && isDragging)
        {
            this.DragMotion();
        }
    }

    private void StartDragging()
    {
        this.isDragging = true;
        this.dragTarget = this.GlobalPosition;
        this.mouseDragStart = GetGlobalMousePosition();
    }

    private void StopDragging()
    {
        this.isDragging = false;
    }

    private void CenterOnMousePosition()
    {
        this.StopDragging();
        this.isDoubleClicked = true;
        this.doubleClickTarget = GetGlobalMousePosition();
    }

    private void DragMotion()
    {
        Vector2 currentMouse = GetGlobalMousePosition();
        Vector2 drag = this.mouseDragStart - currentMouse;
        this.dragTarget += drag;
        this.mouseDragStart = currentMouse;
    }

    public override void _PhysicsProcess(double delta)
    {
        this.UpdateEdgeMoveDirection();
        this.UpdatePosition(delta);
        this.UpdateZoom(delta);
    }

    private void UpdateEdgeMoveDirection()
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector2 viewportSize = GetViewportRect().Size;
        Vector2 viewportCenter = viewportSize / 2;

        this.edgeMoveDirection = Vector2.Zero;

        if ((mousePos.X <= this.EdgeMoveMargin && mousePos.X >= 0)
            || (mousePos.X >= viewportSize.X - this.EdgeMoveMargin && mousePos.X <= viewportSize.X)
            || (mousePos.Y <= this.EdgeMoveMargin && mousePos.Y >= 0)
            || (mousePos.Y >= viewportSize.Y - this.EdgeMoveMargin && mousePos.Y <= viewportSize.Y))
        {
            this.edgeMoveDirection += (mousePos - viewportCenter).Normalized();
        }
    }

    private void UpdatePosition(double delta)
    {
        if (this.isDoubleClicked) this.MoveToDoubleClickPosition(delta);
        else
        {
            if (this.isDragging) this.SmoothDragMovement(delta);
            Vector2 totalMoveDirection = this.moveDirection + this.edgeMoveDirection * this.EdgeMoveSpeedQuantifier;
            this.GlobalPosition += totalMoveDirection * this.MoveSpeed * (float)delta;
        }
    }

    private void UpdateZoom(double delta)
    {
        Vector2 zoomRatio = Vector2.Zero;
        if (this.zoomingIn) zoomRatio += new Vector2(this.ZoomSpeedKeys, this.ZoomSpeedKeys);
        if (this.zoomingInByMouse) zoomRatio += new Vector2(this.ZoomSpeedMouse, this.ZoomSpeedMouse);
        if (this.zoomingOut) zoomRatio -= new Vector2(this.ZoomSpeedKeys, this.ZoomSpeedKeys);
        if (this.zoomingOutByMouse) zoomRatio -= new Vector2(this.ZoomSpeedMouse, this.ZoomSpeedMouse);
        this.Zoom += zoomRatio * (float)delta;
        this.Zoom = this.Zoom.Clamp(new Vector2(this.MinZoom, this.MinZoom), new Vector2(this.MaxZoom, this.MaxZoom));

        this.zoomingInByMouse = false;
        this.zoomingOutByMouse = false;
    }
    private void SmoothDragMovement(double delta)
    {
        if (this.GlobalPosition.DistanceTo(this.dragTarget) > 0.1f)
        {
            this.GlobalPosition = this.GlobalPosition.Lerp(this.dragTarget, this.DragSmoothness);
        }
    }

    private void MoveToDoubleClickPosition(double delta)
    {
        if (this.GlobalPosition.DistanceTo(this.doubleClickTarget) > 0.1f)
        {
            this.GlobalPosition = this.GlobalPosition.Lerp(this.doubleClickTarget, this.DoubleClickSmoothness);
        }
        else
        {
            this.isDoubleClicked = false;
        }
    }
}