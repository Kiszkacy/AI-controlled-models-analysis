
using Godot;

public partial class PauseButton : Button
{
    private Texture2D iconPaused;
    private Texture2D iconRunning;
    private bool inPauseMode = false;

    public override void _Ready()
    {
        this.iconPaused = (Texture2D)GD.Load("res://assets/icons/solid/play.svg");
        this.iconRunning = (Texture2D)GD.Load("res://assets/icons/solid/pause.svg");
    }

    public void Invert()
    {
        this.inPauseMode = !this.inPauseMode;

        if (this.inPauseMode)
        {
            this.Icon = this.iconPaused;
        }
        else
        {
            this.Icon = this.iconRunning;
        }
    }
}