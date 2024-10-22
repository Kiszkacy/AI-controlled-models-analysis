using Godot;

public partial class Window : Control
{
    // Parameters editable in the Godot editor
    [Export] public int Width { get; set; } = 300;
    [Export] public int Height { get; set; } = 200;
    [Export] public string Title { get; set; } = "Window Title";
    [Export] public bool HasCloseButton { get; set; } = true;
    [Export] public bool IsMovable { get; set; } = true;
    [Export] public string Text { get; set; } = "Window Content";

    private Label _titleLabel;
    private Button _closeButton;
    private Label _contentLabel;
    private Panel _windowPanel;
    private bool _dragging = false;
    private Vector2 _dragOffset;

    public override void _Ready()
    {
        // Create the window panel
        // _windowPanel = new Panel();
        // _windowPanel.Size = new Vector2(Width, Height);
        // _windowPanel.CustomMinimumSize = new Vector2(Width, Height);
        // AddChild(_windowPanel);
        //
        // // Create the title bar with label and optional close button
        // HBoxContainer titleBar = new HBoxContainer();
        // titleBar.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        // titleBar.CustomMinimumSize = new Vector2(Width, 30);
        // _windowPanel.AddChild(titleBar);
        //
        // // Title label
        // _titleLabel = new Label();
        // _titleLabel.Text = Title;
        // _titleLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        // titleBar.AddChild(_titleLabel);
        //
        // // Close button (if enabled)
        // if (HasCloseButton)
        // {
        //     _closeButton = new Button();
        //     _closeButton.Text = "X";
        //     _closeButton.Pressed += this.OnCloseButtonPressed;
        //     titleBar.AddChild(_closeButton);
        // }
        //
        // // Create content label
        // _contentLabel = new Label();
        // _contentLabel.Text = Text;
        // _contentLabel.CustomMinimumSize = new Vector2(Width - 20, Height - 40);
        // _windowPanel.AddChild(_contentLabel);
        //
        // // Set the mouse tracking area for dragging (if movable)
        // if (IsMovable)
        // {
        //     _windowPanel.GuiInput += this.OnWindowGuiInput;
        // }
    }

    // Called when the user clicks and drags the window
    private void OnWindowGuiInput(InputEvent @event)
    {
        if (!IsMovable) return;

        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                _dragging = true;
                _dragOffset = mouseEvent.Position;
            }
            else
            {
                _dragging = false;
            }
        }
        else if (@event is InputEventMouseMotion motionEvent && _dragging)
        {
            this.Position += motionEvent.Relative;
        }
    }

    // Handles closing the window
    private void OnCloseButtonPressed()
    {
        QueueFree();
    }

    public override void _Process(double delta)
    {
        // Update parameters dynamically in the editor
        _windowPanel.CustomMinimumSize = new Vector2(Width, Height);
        _titleLabel.Text = Title;
        _contentLabel.Text = Text;
    }
}