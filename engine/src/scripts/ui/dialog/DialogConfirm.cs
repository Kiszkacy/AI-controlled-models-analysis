using System;

using Godot;

public partial class DialogConfirm : Control
{
    [Export(PropertyHint.MultilineText)]
    public string Title { get; set; } = "Are you sure?";
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = "You cannot undo this action.";
    [Export]
    public Color BackgroundColor { get; set; } = new Color(0x20202080);

    [ExportGroup("DO NOT EDIT THESE")]
    [Export]
    private ColorRect DarkenBackground;
    [Export]
    private Label TitleLabel;
    [Export]
    private Label DescriptionLabel;
    [Export]
    private Button CloseButton;
    [Export]
    private Button CancelButton;
    [Export]
    private Button ConfirmButton;


    public event Action Confirmed;
    public event Action Cancelled;

    public override void _Ready()
    {
        this.Setup();
    }

    private void Setup()
    {
        this.TitleLabel.Text = this.Title;
        this.DescriptionLabel.Text = this.Description;
        this.DarkenBackground.Color = this.BackgroundColor;
        this.ConnectButtons();
    }

    private void ConnectButtons()
    {
        this.CloseButton.Pressed += this.OnCloseClick;
        this.CancelButton.Pressed += this.OnCancelClick;
        this.ConfirmButton.Pressed += this.OnConfirmClick;
    }

    private void OnCloseClick()
    {
        this.Close();
    }

    private void OnConfirmClick()
    {
        this.Confirmed?.Invoke();
        this.Close();
    }

    private void OnCancelClick()
    {
        this.Close();
    }

    private void Close()
    {
        this.Cancelled?.Invoke();
        this.Visible = false;
    }
    
    public void Open()
    {
        this.Visible = true;
    }
}