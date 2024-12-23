
using System;

using Godot;

public partial class InputHandler : Control
{
    [Export]
    public string Title { get; set; } = "Input title";
    [Export]
    public string DefaultText { get; set; } = "";
    [Export]
    public bool ShowMiniLabelName { get; set; } = true;
    [Export]
    public bool Disabled { get; set; } = false;

    [ExportGroup("DO NOT EDIT THESE")]
    [Export]
    private Label MiniLabel;
    [Export]
    private LineEdit InputLine;

    private string CurrentText => this.InputLine.Text;
    private string lastSubmittedText = "";
    
    public event EventHandler<BaseEventArgs<string>> TextSubmitted;

    public override void _Ready()
    {
        this.Setup();
    }
    
    private void Setup()
    {
        this.InputLine.PlaceholderText = this.Title;
        this.SetTextTo(this.DefaultText);
        this.MiniLabel.Text = this.Title;
        this.InputLine.Editable = !this.Disabled;
        this.InputLine.SelectingEnabled = !this.Disabled;
        
        this.InputLine.FocusEntered += this.OnFocusGained;
        this.InputLine.FocusExited += this.OnFocusLost;
        this.InputLine.TextSubmitted += this.OnTextSubmitted;
        this.InputLine.TextChanged += this.OnTextChanged;
        
        this.UpdateMiniLabelVisibility();
    }

    private void OnFocusGained()
    {
        this.UpdateMiniLabelVisibility();
    }
    
    private void OnFocusLost()
    {
        this.InputLine.Text = this.lastSubmittedText;
        this.UpdateMiniLabelVisibility();
    }
    
    private void OnTextSubmitted(string _)
    {
        this.lastSubmittedText = this.CurrentText;
        this.UpdateMiniLabelVisibility();
        this.TextSubmitted?.Invoke(this, new BaseEventArgs<string>(this.CurrentText));
    }
    
    private void OnTextChanged(string _)
    {
        this.UpdateMiniLabelVisibility();
    }

    private void UpdateMiniLabelVisibility()
    {
        this.MiniLabel.Visible = this.ShowMiniLabelName && this.CurrentText.Length != 0;
    }

    public void SetTextTo(string value)
    {
        this.InputLine.Text = value;
        this.lastSubmittedText = this.CurrentText;
        this.UpdateMiniLabelVisibility();
    }
}
