using System;
using System.Threading.Tasks;

using Godot;

public partial class Camera : Camera2D, Observable
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

    [Export(PropertyHint.Range, "0.1,100.0,")]
    public float DragSmoothness { get; set; } = 15.0f;

    [Export(PropertyHint.Range, "0.1,100.0,")]
    public float DoubleClickSmoothness { get; set; } = 15.0f;

    [Export(PropertyHint.Range, "10,100,1")]
    public float EdgeMoveMargin { get; set; } = 50f;

    [Export(PropertyHint.Range, "0.5,10.0,0.1")]
    public float EdgeMoveSpeedQuantifier { get; set; } = 3f;

    [Export] public CanvasLayer UILayer;

    private Vector2 moveDirection = Vector2.Zero;
    private Vector2 edgeMoveDirection = Vector2.Zero;
    private bool zoomingIn = false;
    private bool zoomingOut = false;
    private bool zoomingInByMouse = false;
    private bool zoomingOutByMouse = false;
    private bool isDragging = false;
    private bool hasLeftOverDragForce = false;
    private Vector2 dragTarget = Vector2.Zero;
    private Vector2 positionDragStart = Vector2.Zero;
    private Vector2 mouseDragStart = Vector2.Zero;
    private bool isDoubleClicked = false;
    private Vector2 doubleClickTarget = Vector2.Zero;
    private bool isEdgeMoveEnabled = true;
    private bool isFollowing = false;
    private Node2D followTarget = null;

    private Timer dragMotionTimer;
    private const float changeCursorShapeIfDraggingFor = 0.3f; // seconds
    private const float minimalDragDistanceToChangeCursorShape = 4.0f; // in px
    private bool overrideDragCursorShape = false;

    private string screenshotPath;

    [Signal]
    public delegate void ScreenshotSavedEventHandler();

    public override void _Ready()
    {
        if (!Reloader.Get().IsReloading)
        {
            this.Zoom = new Vector2(0.5f, 0.5f);
        }
        this.dragMotionTimer = new Timer(this.DragMotionTimeout);
        this.SetProperties();
        EventManager.Instance.Subscribe(this, EventChannel.ObjectTracker);
        EventManager.Instance.Subscribe(this, EventChannel.Settings);
    }

    private void SetProperties()
    {
        this.ZoomSpeedKeys = Config.Instance.Data.Controls.ZoomSensitivity;
        this.ZoomSpeedMouse = Config.Instance.Data.Controls.ZoomSensitivity * 2.5f;
        this.MaxZoom = Config.Instance.Data.Controls.MaxZoom;
        this.MinZoom = Config.Instance.Data.Controls.MinZoom;
        this.isEdgeMoveEnabled = Config.Instance.Data.Controls.EdgeMoveEnabled;
        this.EdgeMoveMargin = Config.Instance.Data.Controls.EdgeMoveMargin;
        this.EdgeMoveSpeedQuantifier = Config.Instance.Data.Controls.EdgeMoveSpeed;
    }

    public override void _Input(InputEvent @event)
    {
        HandleKeyboardInput(@event);
        HandleMouseInput(@event);

        if (isFollowing && IsUserMovement(@event))
        {
            StopFollowing();
        }
    }

    private void SetLayerChildrenVisibility(Node node, bool visible)
    {
        if (node is CanvasItem canvasItem)
        {
            if (!visible)
            { canvasItem.Visible = false; }
            else
            { canvasItem.Visible = true; }
        }
    }

    private bool IsUserMovement(InputEvent @event)
    {
        return @event.IsActionPressed("move.camera.up") ||
               @event.IsActionPressed("move.camera.down") ||
               @event.IsActionPressed("move.camera.left") ||
               @event.IsActionPressed("move.camera.right") ||
               this.isDragging || this.isDoubleClicked;
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
            switch (mouseEvent.ButtonIndex)
            {
                case MouseButton.Left:
                    if (mouseEvent.IsPressed() && !IsMouseOverUI())
                    {
                        this.StartDragging();
                    }
                    else if (mouseEvent.IsReleased())
                    {
                        this.StopDragging();
                    }

                    if (mouseEvent.DoubleClick && !IsMouseOverUI())
                    {
                        this.CenterOnMousePosition();
                    }
                    break;
                case MouseButton.WheelUp:
                    this.zoomingInByMouse = true;
                    break;
                case MouseButton.WheelDown:
                    this.zoomingOutByMouse = true;
                    break;
            }
        }
        else if (@event is InputEventMouseMotion motionEvent && isDragging)
        {
            this.DragMotion();
        }
    }

    private void StartDragging()
    {
        if (this.isDragging)
        {
            return;
        }

        this.isDragging = true;
        this.mouseDragStart = this.GetViewport().GetMousePosition();
        this.positionDragStart = this.GlobalPosition;
        this.dragTarget = this.positionDragStart;
        this.dragMotionTimer.Activate(changeCursorShapeIfDraggingFor);
    }

    private void StopDragging()
    {
        if (!this.isDragging)
        {
            return;
        }

        this.dragMotionTimer.Stop();
        this.isDragging = false;
        Vector2 dragLeftOverForce = this.dragTarget - this.GlobalPosition;
        this.hasLeftOverDragForce = dragLeftOverForce.Length() >= 1.0f;
        this.overrideDragCursorShape = false;
    }

    public void MoveTo(Vector2 position)
    {
        this.StopDragging();
        this.isDoubleClicked = true;
        this.doubleClickTarget = position;
    }

    private void CenterOnMousePosition()
    {
        this.MoveTo(GetGlobalMousePosition());
    }

    private void DragMotion()
    {
        Vector2 currentMouse = this.GetViewport().GetMousePosition();
        Vector2 drag = this.mouseDragStart - currentMouse;
        this.dragTarget = this.positionDragStart + drag * this.Zoom.Inverse();
    }

    public override void _Process(double delta)
    {
        this.dragMotionTimer.Process(delta);

        if (this.isFollowing && this.followTarget != null)
        {
            this.dragTarget = this.followTarget.GlobalPosition;
            this.SmoothDragMovement(delta);
        }
        else
        {
            if (isEdgeMoveEnabled)
            {
                this.UpdateEdgeMoveDirection();
            }
            this.UpdatePosition(delta);
        }

        if (!IsMouseOverUI())
        {
            this.UpdateZoom(delta);
            this.UpdateCursorShape();
        }
    }

    private bool IsMouseOverUI()
    {
        Control hoveredNode = GetViewport().GuiGetHoveredControl();

        if (hoveredNode != null && UILayer.IsAncestorOf(hoveredNode))
        {

            if (hoveredNode.GetParent() is Collapsible collapsible)
            {
                return collapsible.IsExpanded();
            }

            return true;
        }
        return false;
    }

    private void UpdateEdgeMoveDirection()
    {
        if (IsMouseOverUI())
        {
            this.edgeMoveDirection = Vector2.Zero;
            return;
        }
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
        else if (this.isDragging) this.SmoothDragMovement(delta);
        else if (this.hasLeftOverDragForce)
        {
            Vector2 dragLeftOverForce = this.dragTarget - this.GlobalPosition;
            if (dragLeftOverForce.Length() >= 1.0f)
            {
                this.SmoothDragMovement(delta);
                this.dragTarget.Lerp(Vector2.Zero, Mathf.Min(0.5f, DragSmoothness*(float)delta));
            }
            else
            {
                this.dragTarget = Vector2.Zero;
                this.hasLeftOverDragForce = false;
            }
        }
        else
        {
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
        Vector2 target = this.dragTarget;
        if (this.isFollowing)
        {
            target = this.followTarget.GlobalPosition;
        }

        this.GlobalPosition = this.GlobalPosition.DistanceTo(target) > 0.1f
            ? this.GlobalPosition.Lerp(target, Mathf.Min(0.5f, this.DragSmoothness * (float)delta))
            : target;
    }

    private void MoveToDoubleClickPosition(double delta)
    {
        if (this.GlobalPosition.DistanceTo(this.doubleClickTarget) > 0.1f)
        {
            this.GlobalPosition = this.GlobalPosition.Lerp(this.doubleClickTarget, Mathf.Min(0.5f, this.DoubleClickSmoothness * (float)delta));
        }
        else
        {
            this.GlobalPosition = this.doubleClickTarget;
            this.isDoubleClicked = false;
        }
    }

    public void Follow(Node2D objectToFollow)
    {
        this.isFollowing = true;
        this.followTarget = objectToFollow;
    }

    public void StopFollowing()
    {
        this.isFollowing = false;
        this.followTarget = null;
    }

    private void UpdateCursorShape()
    {
        if ((this.isDragging && this.mouseDragStart.DistanceTo(this.GetViewport().GetMousePosition()) >= minimalDragDistanceToChangeCursorShape) || this.overrideDragCursorShape)
        {
            Input.SetDefaultCursorShape(Input.CursorShape.Drag);
            this.overrideDragCursorShape = true;
        }
        else
        {
            Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
        }
    }

    private void DragMotionTimeout()
    {
        this.overrideDragCursorShape = true;
    }

    public async Task TakeScreenshot(string screenshotPath)
    {
        UILayer.Visible = false;
        this.screenshotPath = screenshotPath;
        RenderingServer.FramePostDraw += OnFramePostDrawScreenshot;
        await ToSignal(this, "ScreenshotSaved");
    }

    private void OnFramePostDrawScreenshot()
    {
        RenderingServer.FramePostDraw -= OnFramePostDrawScreenshot;
        var viewport = GetViewport();
        var img = viewport.GetTexture().GetImage();
        UILayer.Visible = true;
        if (img.SavePng(this.screenshotPath) != Error.Ok)
        {
            NeatPrinter.Start()
                .ColorPrint(ConsoleColor.Red, "[CAMERA]")
                .Print("  | Failed to save screenshot to " + this.screenshotPath)
                .End();
        }
        EmitSignal("ScreenshotSaved");
    }

    public void Notify(IEvent @event)
    {
        if (@event is NodeEvent nodeEvent)
        {
            if (nodeEvent.Node != null)
            {
                this.Follow((Node2D)nodeEvent.Node);
            }
            else if (nodeEvent.Node == null && this.isFollowing)
            {
                this.StopFollowing();
            }
        }
        else if (@event is NotifyEvent settingsEvent)
        {
            this.SetProperties();
        }
    }
}