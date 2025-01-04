using System;

using Godot;

public partial class ActionHandler : Node2D
{
    private LineEdit _textInput;

    public override void _Ready()
    {
        _textInput = GetNode<LineEdit>("TextInput");
        _textInput.Visible = false;
        _textInput.TextSubmitted += OnTextSubmitted;
        _textInput.ZIndex = 2;
    }

    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("load.main"))
        {
            Reloader.Get().IsReloading = true;
            PackedScene mainScene = ResourceLoader.Load<PackedScene>("res://src/scenes/main.tscn");
            GetTree().ChangeSceneToPacked(mainScene);
        }
        else if (@event.IsActionPressed("text.input"))
        {
            _textInput.Visible = true;
            _textInput.GrabFocus();
        }
    }

    private void OnTextSubmitted(string text)
    {
        _textInput.Visible = false;
        Reloader.Get().SetSaveFileName(text);
    }

}