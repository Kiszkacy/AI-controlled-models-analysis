using Godot;

public partial class HudManager : CanvasLayer
{
    // TODO this is temporary
    [Export]
    public Button ExitButton;

    public override void _Ready()
    {
        this.ConnectButtons();
    }

    private void ConnectButtons()
    {
        this.ExitButton.Pressed += this.OnExitClick;
    }

    private void OnExitClick()
    {
        this.GetTree().Quit();
    }
}