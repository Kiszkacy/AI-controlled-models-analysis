
using System.Linq;

using Godot;

public partial class AnalysisUI : Control, Observable
{
    [Export]
    public EnvironmentTracker EnvironmentTracker;
    [Export]
    public Chart AgentCountChart;
    [Export]
    public Chart AgentsMeanEnergyChart;
    [Export]
    public Chart AgentsEnergySumChart;
    [Export]
    public Chart EnvironmentTotalEnergyChart;
    [Export]
    public Chart FoodToAgentsRatioChart;
    [Export]
    public Chart FoodCountChart;
    
    [Export]
    public Button SaveDataButton;
    [Export]
    public Button ClearDataButton;

    public override void _Ready()
    {
        EventManager.Instance.Subscribe(this, EventChannel.EnvironmentTracker);
        this.ConnectButtons();
    }

    private void ConnectButtons()
    {
        this.SaveDataButton.Pressed += this.OnSaveDataClick;
        this.ClearDataButton.Pressed += this.OnClearDataClick;
    }

    private void OnSaveDataClick()
    {
        // TODO open file explorer and create a csv file from existing data
    }

    private void OnClearDataClick()
    {
        this.EnvironmentTracker.ClearCacheData();
        this.FillChartsWithEmptyData();
    }

    public void Notify(IEvent @event)
    {
        this.UpdateCharts();
    }

    private void UpdateCharts()
    {
        this.AgentCountChart.UpdateChartData(
            this.EnvironmentTracker.AgentsCountData.Zip(this.EnvironmentTracker.TimeData, (agentCount, time) => new Vector2((float)time, agentCount)).ToArray(),
            new Vector2((float)this.EnvironmentTracker.TimeData.Min(), 0),
            new Vector2((float)this.EnvironmentTracker.TimeData.Max(), 100),
            new Vector2(5, 5)
        );
        
        this.AgentsMeanEnergyChart.UpdateChartData(
            this.EnvironmentTracker.AgentsMeanEnergyData.Zip(this.EnvironmentTracker.TimeData, (agentEnergy, time) => new Vector2((float)time, agentEnergy)).ToArray(),
            new Vector2((float)this.EnvironmentTracker.TimeData.Min(), 0),
            new Vector2((float)this.EnvironmentTracker.TimeData.Max(), 200),
            new Vector2(5, 5)
        );
        
        this.AgentsEnergySumChart.UpdateChartData(
            this.EnvironmentTracker.AgentsEnergySumData.Zip(this.EnvironmentTracker.TimeData, (agentEnergy, time) => new Vector2((float)time, agentEnergy)).ToArray(),
            new Vector2((float)this.EnvironmentTracker.TimeData.Min(), 0),
            new Vector2((float)this.EnvironmentTracker.TimeData.Max(), 10000),
            new Vector2(5, 5)
        );
        
        this.EnvironmentTotalEnergyChart.UpdateChartData(
            this.EnvironmentTracker.EnvironmentTotalEnergyData.Zip(this.EnvironmentTracker.TimeData, (energy, time) => new Vector2((float)time, energy)).ToArray(),
            new Vector2((float)this.EnvironmentTracker.TimeData.Min(), 0),
            new Vector2((float)this.EnvironmentTracker.TimeData.Max(), 50000),
            new Vector2(5, 5)
        );
        
        this.FoodToAgentsRatioChart.UpdateChartData(
            this.EnvironmentTracker.FoodToAgentsRatioData.Zip(this.EnvironmentTracker.TimeData, (ratio, time) => new Vector2((float)time, ratio)).ToArray(),
            new Vector2((float)this.EnvironmentTracker.TimeData.Min(), 0),
            new Vector2((float)this.EnvironmentTracker.TimeData.Max(), 20),
            new Vector2(5, 5)
        );
        
        this.FoodCountChart.UpdateChartData(
            this.EnvironmentTracker.FoodCountData.Zip(this.EnvironmentTracker.TimeData, (amount, time) => new Vector2((float)time, amount)).ToArray(),
            new Vector2((float)this.EnvironmentTracker.TimeData.Min(), 0),
            new Vector2((float)this.EnvironmentTracker.TimeData.Max(), 1000),
            new Vector2(5, 5)
        );
    }

    private void FillChartsWithEmptyData()
    {
        this.AgentCountChart.UpdateChartData(
            new Vector2[]{},
            Vector2.Zero,
            Vector2.Zero,
            Vector2.Zero
        );
        
        this.AgentsMeanEnergyChart.UpdateChartData(
            new Vector2[]{},
            Vector2.Zero,
            Vector2.Zero,
            Vector2.Zero
        );
        
        this.AgentsEnergySumChart.UpdateChartData(
            new Vector2[]{},
            Vector2.Zero,
            Vector2.Zero,
            Vector2.Zero
        );
        
        this.EnvironmentTotalEnergyChart.UpdateChartData(
            new Vector2[]{},
            Vector2.Zero,
            Vector2.Zero,
            Vector2.Zero
        );
        
        this.FoodToAgentsRatioChart.UpdateChartData(
            new Vector2[]{},
            Vector2.Zero,
            Vector2.Zero,
            Vector2.Zero
        );
        
        this.FoodCountChart.UpdateChartData(
            new Vector2[]{},
            Vector2.Zero,
            Vector2.Zero,
            Vector2.Zero
        );
    }
}
