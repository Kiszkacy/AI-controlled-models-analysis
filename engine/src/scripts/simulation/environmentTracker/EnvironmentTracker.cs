
using System.Collections.Generic;
using System.Linq;

using Godot;

public partial class EnvironmentTracker : Node
{
    [Export] public float CacheIntervalSeconds = 5f;
    [Export] public int MaxDataPoints = 60;
    private Timer cacheTimer;
    public List<int> AgentsCountData = new();
    public List<float> AgentsMeanEnergyData = new();
    public List<float> AgentsEnergySumData = new();
    public List<float> EnvironmentTotalEnergyData = new();
    public List<float> FoodToAgentsRatioData = new();
    public List<int> FoodCountData = new();
    public List<double> TimeData = new();

    public override void _Ready()
    {
        this.cacheTimer = new Timer(this.CacheData);
        this.cacheTimer.Activate(this.CacheIntervalSeconds);
    }

    public override void _PhysicsProcess(double delta)
    {
        this.cacheTimer.Process(delta);
    }

    public void ClearCacheData()
    {
        this.AgentsCountData.Clear();
        this.AgentsMeanEnergyData.Clear();
        this.AgentsEnergySumData.Clear();
        this.EnvironmentTotalEnergyData.Clear();
        this.FoodToAgentsRatioData.Clear();
        this.FoodCountData.Clear();
        this.TimeData.Clear();
    }

    private void CacheData()
    {
        this.AgentsCountData.Add(this.AgentsCount());
        this.AgentsMeanEnergyData.Add(this.AgentsMeanEnergy());
        this.AgentsEnergySumData.Add(this.AgentsEnergySum());
        this.EnvironmentTotalEnergyData.Add(this.EnvironmentTotalEnergy());
        this.FoodToAgentsRatioData.Add(this.FoodToAgentsRatio());
        this.FoodCountData.Add(this.FoodCount());
        this.TimeData.Add(SimulationManager.Instance.TimePassed);
        this.TrimData();

        EventManager.Instance.RegisterEvent(new NotifyEvent(null), EventChannel.EnvironmentTracker);
        this.cacheTimer.Activate(this.CacheIntervalSeconds);
    }

    private void TrimData()
    {
        if (this.AgentsCountData.Count > this.MaxDataPoints)
        {
            this.AgentsCountData.RemoveRange(0, this.AgentsCountData.Count - this.MaxDataPoints);
        }
        if (this.AgentsMeanEnergyData.Count > this.MaxDataPoints)
        {
            this.AgentsMeanEnergyData.RemoveRange(0, this.AgentsMeanEnergyData.Count - this.MaxDataPoints);
        }
        if (this.AgentsEnergySumData.Count > this.MaxDataPoints)
        {
            this.AgentsEnergySumData.RemoveRange(0, this.AgentsEnergySumData.Count - this.MaxDataPoints);
        }
        if (this.EnvironmentTotalEnergyData.Count > this.MaxDataPoints)
        {
            this.EnvironmentTotalEnergyData.RemoveRange(0, this.EnvironmentTotalEnergyData.Count - this.MaxDataPoints);
        }
        if (this.FoodToAgentsRatioData.Count > this.MaxDataPoints)
        {
            this.FoodToAgentsRatioData.RemoveRange(0, this.FoodToAgentsRatioData.Count - this.MaxDataPoints);
        }
        if (this.FoodCountData.Count > this.MaxDataPoints)
        {
            this.FoodCountData.RemoveRange(0, this.FoodToAgentsRatioData.Count - this.MaxDataPoints);
        }
        if (this.TimeData.Count > this.MaxDataPoints)
        {
            this.TimeData.RemoveRange(0, this.TimeData.Count - this.MaxDataPoints);
        }
    }

    private int AgentsCount()
    {
        return AgentManager.Instance.Agents.Count;
    }

    private float AgentsMeanEnergy()
    {
        return this.AgentsEnergySum() / this.AgentsCount();
    }

    private float AgentsEnergySum()
    {
        return AgentManager.Instance.Agents.Values.Sum(agent => agent.energy);
    }

    private float EnvironmentTotalEnergy()
    {
        float foodEnergy = 0f;
        EntityManager.Instance.FoodBuckets.ForEachEntity(entity => foodEnergy += entity.EnergyNutrition);
        return foodEnergy + this.AgentsEnergySum();
    }

    private float FoodToAgentsRatio()
    {
        return (float)this.FoodCount() / this.AgentsCount();
    }

    private int FoodCount()
    {
        int foodCount = 0;
        EntityManager.Instance.FoodBuckets.ForEachEntity(_ => foodCount++);
        return foodCount;
    }
}