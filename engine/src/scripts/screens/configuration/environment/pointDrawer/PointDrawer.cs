using System.Collections.Generic;

using Godot;

public partial class PointDrawer : Control
{
    [Export]
    private Color PointColor { get; set; } = new(1, 0, 0);
    [Export]
    private float PointRadius { get; set; } = 2.0f;
    
    
    private LinkedList<Vector2> points = new();

    public void AddPoint(Vector2 point)
    {
        this.points.AddLast(point);
    }

    public void ClearPoints()
    {
        this.points.Clear();
    }
    
    public override void _Draw()
    {
        foreach (Vector2 point in this.points)
        {
            this.DrawCircle(point, this.PointRadius, this.PointColor);
        }
    }
}