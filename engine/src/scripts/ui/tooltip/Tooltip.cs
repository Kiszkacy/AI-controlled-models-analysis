using Godot;

public enum TooltipLayout
{
    Top,
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft,
    Left,
    TopLeft,
}

public partial class Tooltip : Control
{
    [Export]
    public int MaxWidth = 320;

    [Export(PropertyHint.MultilineText)]
    public string Text
    {
        get => this.text;
        set // setter in case if the text changes while tooltip is visible, idk why would we need this but its here
        {
            this.text = value;
            if (this.isReady)
            { // isReady is in case exported parameter is set not to a default value, godot runs this code before _ready is called !!
                this.Set(text);
            }
        }
    }

    [Export]
    public NodePath TargetNodePath;

    [Export]
    public TooltipLayout Layout = TooltipLayout.Top;

    [Export]
    public HorizontalAlignment TextAlignment = HorizontalAlignment.Center;

    // TODO maybe add this in the future ?
    // [Export] 
    // public bool MoveWithMouse = true;

    [Export]
    public bool ShowArrow = true;

    [Export]
    public float MakeVisibleAfterHoveringFor = 1.0f; // in seconds

    private Label label;
    private Label background;
    private Control arrow;
    private Control targetNode;
    private string text = string.Empty;
    private bool isReady = false;

    private readonly int paddingVertical = 4;
    private readonly int paddingHorizontal = 8;

    private readonly Timer hoverTimer;

    public override void _Ready()
    {
        this.label = this.GetNode<Label>("Label");
        this.background = this.GetNode<Label>("Background");
        this.label.HorizontalAlignment = this.TextAlignment;
        this.background.HorizontalAlignment = this.TextAlignment;

        this.arrow = this.GetNode<Control>("Arrow");
        if (!this.ShowArrow)
        {
            this.arrow.Visible = false;
        }

        this.Set(this.Text);
        this.Visible = false;
        this.isReady = true;
        this.TryToGetTargetNode();
    }

    private void TryToGetTargetNode()
    {
        if (string.IsNullOrEmpty(this.TargetNodePath) || this.targetNode != null)
        {
            return;
        }

        this.targetNode = this.GetNode<Control>(this.TargetNodePath);
        if (this.targetNode != null)
        {
            this.targetNode.MouseEntered += this.OnMouseEntered;
            this.targetNode.MouseExited += this.OnMouseExited;
        }
    }

    public void Set(string text)
    {
        Vector2 textSize = this.label.GetThemeDefaultFont().GetMultilineStringSize(
            text,
            this.TextAlignment,
            this.MaxWidth-paddingHorizontal*2,
            brkFlags: TextServer.LineBreakFlag.WordBound | TextServer.LineBreakFlag.Adaptive | TextServer.LineBreakFlag.Mandatory,
            justificationFlags: TextServer.JustificationFlag.Kashida | TextServer.JustificationFlag.WordBound | TextServer.JustificationFlag.SkipLastLine | TextServer.JustificationFlag.DoNotSkipSingleLine
        );

        this.label.Text = string.Empty;
        this.background.Text = string.Empty;

        this.label.Size = new Vector2(textSize.X+paddingHorizontal*2, textSize.Y+paddingVertical*2);
        this.background.Size = new Vector2(textSize.X+paddingHorizontal*2, textSize.Y+paddingVertical*2);
        this.Size = new Vector2(textSize.X+paddingHorizontal*2, textSize.Y+paddingVertical*2);

        this.label.Text = text;
        this.background.Text = text;

        this.UpdateArrowPosition();
    }

    private void OnMouseEntered()
    {
        this.hoverTimer.Activate(this.MakeVisibleAfterHoveringFor);
    }

    private void OnMouseExited()
    {
        this.hoverTimer.Stop();
        this.Visible = false;
    }

    public override void _Process(double delta)
    {
        this.hoverTimer.Process(delta);

        if (!this.Visible || this.targetNode == null)
        {
            return;
        }

        Vector2 mousePosition = GetViewport().GetMousePosition();
        switch (this.Layout)
        {
            case TooltipLayout.Top:
                this.Position = new Vector2(mousePosition.X - this.label.Size.X / 2, mousePosition.Y - 16 - this.label.Size.Y + (this.ShowArrow ? 0 : 10));
                break;
            case TooltipLayout.TopRight:
                this.Position = new Vector2(mousePosition.X - 22, mousePosition.Y - 16 - this.label.Size.Y + (this.ShowArrow ? 0 : 10));
                break;
            case TooltipLayout.Right:
                this.Position = new Vector2(mousePosition.X + 24 - (this.ShowArrow ? 0 : 10), mousePosition.Y - this.label.Size.Y/2);
                break;
            case TooltipLayout.BottomRight:
                this.Position = new Vector2(mousePosition.X - 22, mousePosition.Y + 32 - (this.ShowArrow ? 0 : 10));
                break;
            case TooltipLayout.Bottom:
                this.Position = new Vector2(mousePosition.X - this.label.Size.X / 2, mousePosition.Y + 32 - (this.ShowArrow ? 0 : 10));
                break;
            case TooltipLayout.BottomLeft:
                this.Position = new Vector2(mousePosition.X - this.Size.X + 26, mousePosition.Y + 32 - (this.ShowArrow ? 0 : 10));
                break;
            case TooltipLayout.Left:
                this.Position = new Vector2(mousePosition.X - this.Size.X - 16 + (this.ShowArrow ? 0 : 10), mousePosition.Y - this.label.Size.Y/2);
                break;
            case TooltipLayout.TopLeft:
                this.Position = new Vector2(mousePosition.X - this.Size.X + 26, mousePosition.Y - 16 - this.label.Size.Y + (this.ShowArrow ? 0 : 10));
                break;
        }
    }

    public void UpdateArrowPosition()
    {
        if (!ShowArrow)
        {
            return;
        }

        float labelSizeX = this.label.Size.X;
        float labelSizeY = this.label.Size.Y;

        int lineHeight = (int)this.label.GetThemeDefaultFont().GetStringSize("text").Y;
        int lineCount = (int)(labelSizeY / lineHeight);

        switch (this.Layout)
        {
            case TooltipLayout.Top:
                this.arrow.Position = new Vector2(labelSizeX / 2, lineCount*23-7);
                break;
            case TooltipLayout.TopRight:
                this.arrow.Position = new Vector2(24, lineCount*23-7);
                break;
            case TooltipLayout.Right:
                this.arrow.Position = new Vector2(1, (int)(lineCount*11.5-8.5));
                break;
            case TooltipLayout.BottomRight:
                this.arrow.Position = new Vector2(24, -11);
                break;
            case TooltipLayout.Bottom:
                this.arrow.Position = new Vector2(labelSizeX/2, -11);
                break;
            case TooltipLayout.BottomLeft:
                this.arrow.Position = new Vector2(labelSizeX-24, -11);
                break;
            case TooltipLayout.Left:
                this.arrow.Position = new Vector2(labelSizeX-1, (int)(lineCount*11.5-8.5));
                break;
            case TooltipLayout.TopLeft:
                this.arrow.Position = new Vector2(labelSizeX-24, lineCount*23-7);
                break;
        }
    }

    private void OnHoverTimeout()
    {
        this.Visible = true;
    }

    public Tooltip()
    {
        this.hoverTimer = new Timer(this.OnHoverTimeout);
    }
}