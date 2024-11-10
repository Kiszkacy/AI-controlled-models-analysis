
using Godot;

public partial class Chart : Control
{
    [Export(PropertyHint.MultilineText)]
    public string TitleChart { get; set; } = "Chart title";
    [Export(PropertyHint.MultilineText)]
    public string TitleAxisY { get; set; } = "Axis Y title";
    [Export(PropertyHint.MultilineText)]
    public string TitleAxisX { get; set; } = "Axis X title";

    [ExportGroup("DO NOT EDIT THESE")]
    [Export]
    public Label ChartTitle;
    [Export]
    public Label AxisXTitle;
    [Export]
    public Label AxisYTitle;
    [Export]
    public Line2D Line;
    [Export]
    public Control PointsBackground;
    [Export]
    public HBoxContainer TicksAxisX;
    [Export]
    public VBoxContainer TicksAxisY;
    [Export]
    public Control Lines;

    private Vector2 chartPointDrawingSize => this.PointsBackground.Size;

    private Vector2 currentTickCount = new(-1, -1);

    public override void _Ready()
    {
        this.ChartTitle.Text = this.TitleChart;
        this.AxisXTitle.Text = this.TitleAxisX;
        this.AxisYTitle.Text = this.TitleAxisY;
    }

    public void UpdateChartData(Vector2[] points, Vector2 minimumValues, Vector2 maximumValues, Vector2 targetTickCount)
    {
        this.UpdatePointData(points, minimumValues, maximumValues);
        this.UpdateTickLabels(targetTickCount, minimumValues, maximumValues);
        this.UpdateLines(targetTickCount);
    }

    private void UpdatePointData(Vector2[] points, Vector2 minimumValues, Vector2 maximumValues)
    {
        this.Line.ClearPoints();
        foreach (Vector2 point in points)
        {
            Vector2 normalizedPositionOnChart = (point - minimumValues) / (maximumValues - minimumValues);
            Vector2 positionOnChart = normalizedPositionOnChart * this.chartPointDrawingSize;
            Vector2 invertedPositionOnChart = new(positionOnChart.X, this.chartPointDrawingSize.Y - positionOnChart.Y);
            this.Line.AddPoint(invertedPositionOnChart);
        }
    }

    private void UpdateTickLabels(Vector2 targetTickCount, Vector2 minimumValues, Vector2 maximumValues)
    {
        if (targetTickCount != this.currentTickCount)
        {
            this.TicksAxisX.RemoveAllChildren();
            this.TicksAxisY.RemoveAllChildren();
        }


        for (int index = 0; index < targetTickCount.X; index++)
        {
            Label label = this.CreateNewLabel();
            float ratio = ((1.0f/targetTickCount.X) / 2) + (1.0f/targetTickCount.X) * index;
            label.Text = (minimumValues.X + (maximumValues.X-minimumValues.X) * ratio).ToString("0");
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.CustomMinimumSize = new Vector2(this.chartPointDrawingSize.X / targetTickCount.X, 0);
            this.TicksAxisX.AddChild(label);
        }

        for (int index = (int)targetTickCount.Y-1; index >= 0; index--)
        {
            Label label = this.CreateNewLabel();
            float ratio = ((1.0f/targetTickCount.Y) / 2) + (1.0f/targetTickCount.Y) * index;
            label.Text = (minimumValues.Y + (maximumValues.Y-minimumValues.Y) * ratio).ToString("0");
            label.HorizontalAlignment = HorizontalAlignment.Right;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.CustomMinimumSize = new Vector2(48, this.chartPointDrawingSize.Y / targetTickCount.Y);
            this.TicksAxisY.AddChild(label);
        }
    }

    private void UpdateLines(Vector2 targetTickCount)
    {
        this.Lines.RemoveAllChildren();

        for (int index = 0; index < targetTickCount.Y; index++)
        {
            ColorRect line = this.CreateNewColorRect();
            line.Size = new Vector2(this.chartPointDrawingSize.X, 2);
            float stepSize = this.chartPointDrawingSize.Y / targetTickCount.Y;
            line.Position = new Vector2(0, stepSize*(index+0.5f));
            this.Lines.AddChild(line);
        }

        for (int index = 0; index < targetTickCount.X; index++)
        {
            ColorRect line = this.CreateNewColorRect();
            line.Size = new Vector2(2, this.chartPointDrawingSize.Y);
            float stepSize = this.chartPointDrawingSize.X / targetTickCount.X;
            line.Position = new Vector2(stepSize*(index+0.5f), 0);
            this.Lines.AddChild(line);
        }
    }

    private Label CreateNewLabel()
    {
        Label label = new();
        label.Theme = (Theme)GD.Load("res://assets/ui/themes/whiteTheme.tres");
        label.AddThemeStyleboxOverride("normal", new StyleBoxEmpty());
        return label;
    }

    private ColorRect CreateNewColorRect()
    {
        ColorRect colorRect = new();
        colorRect.Color = new Color(0x434343);
        return colorRect;
    }
}