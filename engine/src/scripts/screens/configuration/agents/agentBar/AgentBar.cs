
using System;

using Godot;

public enum Status
{
    Active,
    Inactive,
    Empty,
    Hidden
}

public partial class AgentBar : Control
{
    private Status status = Status.Empty;

    public Status Status
    {
        get
        {
            return this.status;
        }
        set
        {
            this.status = value;
            this.UpdateBackgroundVisibility();
        }
    }

    [Export]
    public Control InactiveBackground { get; set; }
    [Export]
    public Control ActiveBackground { get; set; }
    [Export]
    public Control EmptyBackground { get; set; }
    [Export]
    public Control NotEmptyParentNode { get; set; }
    [Export]
    public Label NameLabel { get; set; }
    [Export]
    public Label EnergyLabel { get; set; }
    [Export]
    public ButtonHandler TrashButton { get; set; }
    [Export]
    public Button ClickButton { get; set; }
    [Export]
    public Sprite2D Sprite { get; set; }

    public Action Pressed;
    public Action DeleteButtonPressed;

    public AgentTemplate agentData = new();

    public override void _Ready()
    {
        this.UpdateBackgroundVisibility();
        this.TrashButton.Pressed += this.OnDeleteButtonClick;
        this.ClickButton.Pressed += () =>
        {
            if (this.status == Status.Empty || this.Status == Status.Inactive)
            {
                this.Pressed?.Invoke();
            }
        };
    }

    public void UpdateBackgroundVisibility()
    {
        this.InactiveBackground.Visible = this.Status == Status.Inactive;
        this.ActiveBackground.Visible = this.Status == Status.Active;
        this.EmptyBackground.Visible = this.Status == Status.Empty;
        this.NotEmptyParentNode.Visible = this.Status != Status.Empty && this.Status != Status.Hidden;
    }

    private void OnDeleteButtonClick()
    {
        this.DeleteButtonPressed?.Invoke();
    }

    public void ResetData()
    {
        this.agentData = new();
    }
}