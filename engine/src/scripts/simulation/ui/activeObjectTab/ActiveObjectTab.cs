
using Godot;

public partial class ActiveObjectTab : TabContainer, Observable, Collapsible
{
    [Export]
    public Button Icon;
    [Export]
    public Label Title;
    [Export]
    public Label Description;

    private Texture2D iconTree;
    private Texture2D iconBush;
    private Texture2D iconRock;
    private Texture2D iconAgent;
    private Texture2D iconFood;
    private Texture2D iconUnknown;

    private Trackable activeObject = null;
    private bool stickingOut = false;

    public bool HasActiveObject => this.activeObject != null;

    public override void _Ready()
    {
        EventManager.Instance.Subscribe(this, EventChannel.ObjectTracker);
        this.iconTree = (Texture2D)GD.Load("res://assets/icons/solid/tree.svg");
        this.iconBush = (Texture2D)GD.Load("res://assets/icons/solid/leaf.svg");
        this.iconRock = (Texture2D)GD.Load("res://assets/icons/solid/mound.svg");
        this.iconAgent = (Texture2D)GD.Load("res://assets/icons/solid/bugs.svg");
        this.iconFood = (Texture2D)GD.Load("res://assets/icons/solid/apple-whole.svg");
        this.iconUnknown = (Texture2D)GD.Load("res://assets/icons/solid/circle-info.svg");

        this.TabClicked += (@event) => this.OnTabClicked();

    }

    private void OnTabClicked()
    {
        this.stickingOut = !this.stickingOut;

        if (this.stickingOut)
        {
            this.Position += new Vector2(-244, 0);
        }
        else
        {
            this.Position += new Vector2(244, 0);
        }
    }

    public void Notify(IEvent @event)
    {
        if (@event is NodeEvent nodeEvent)
        {
            this.OnActiveObjectChange(nodeEvent.Node);
        }
    }

    private void OnActiveObjectChange(Node activeObject)
    {
        if (activeObject is not Trackable) // if activeObject == null this will be true
        {
            this.Icon.Icon = this.iconUnknown;
            this.Title.Text = "No active object";
            this.Description.Text = "";
            this.activeObject = null;
            return;
        }

        this.activeObject = (Trackable)activeObject;

        if (activeObject is Tree)
        {
            this.Icon.Icon = this.iconTree;
            this.Title.Text = "Tree";
        }
        else if (activeObject is Bush)
        {
            this.Icon.Icon = this.iconBush;
            this.Title.Text = "Bush";
        }
        else if (activeObject is Rock)
        {
            this.Icon.Icon = this.iconRock;
            this.Title.Text = "Rock";
        }
        else if (activeObject is Agent)
        {
            this.Icon.Icon = this.iconAgent;
            this.Title.Text = "Agent";
        }
        else if (activeObject is Food)
        {
            this.Icon.Icon = this.iconFood;
            this.Title.Text = "Food";
        }
    }

    public override void _Process(double delta)
    {
        this.UpdateDescription();
    }

    private void UpdateDescription()
    {
        if (this.HasActiveObject)
        {
            this.Description.Text = string.Join("\n", this.activeObject.GetInformation());
        }
    }

    public bool IsExpanded()
    {
        return this.stickingOut;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed)
            {
                this.OnTabClicked();
            }
        }

    }
}