
using System.Linq;

using Godot;

public partial class AnalysisUI : Control, Observable
{
    [Export] 
    public EnvironmentTracker EnvironmentTracker;
    
    [Export] 
    public Chart AgentChart;

    public override void _Ready()
    {
        EventManager.Instance.Subscribe(this, EventChannel.EnvironmentTracker);
    }

    public void Notify(IEvent @event)
    {
        this.AgentChart.UpdateChartData(
            this.EnvironmentTracker.AgentsCountData.Zip(this.EnvironmentTracker.TimeData, (agentCount, time) => new Vector2((float)time, agentCount)).ToArray(),
            new Vector2((float)this.EnvironmentTracker.TimeData.Min(), 0),
            new Vector2((float)this.EnvironmentTracker.TimeData.Max(), 100),
            new Vector2(5, 5)
        );
    }
}