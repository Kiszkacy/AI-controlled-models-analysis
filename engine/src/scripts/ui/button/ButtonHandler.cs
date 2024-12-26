using System;

using Godot;

public partial class ButtonHandler : Button
{
    private CursorShape defaultCursorShape;

    public override void _Ready()
    {
        this.defaultCursorShape = this.GetDefaultCursorShape();
        this.UpdateAfterDisabledChange();
    }

    public void UpdateAfterDisabledChange()
    { // TODO check if this can be binded to Disabled setter
        if (this.Disabled)
        {
            // Reset to default arrow cursor if the button is disabled
            this.SetDefaultCursorShape(CursorShape.Arrow);
        }
        else
        {
            this.SetDefaultCursorShape(this.defaultCursorShape);
        }
    }
}
