
using System.Collections.Generic;

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
    public List<ulong> TimeData = new();

    public override void _Ready()
    {
        this.cacheTimer = new Timer(this.CacheData);
        this.cacheTimer.Activate(this.CacheIntervalSeconds);
    }

    public override void _PhysicsProcess(double delta)
    {
        this.cacheTimer.Process(delta);
    }

    private void CacheData()
    {
        AgentsCountData.Add(AgentsCount());
        AgentsMeanEnergyData.Add(AgentsMeanEnergy());
        AgentsEnergySumData.Add(AgentsEnergySum());
        EnvironmentTotalEnergyData.Add(EnvironmentTotalEnergy());
        FoodToAgentsRatioData.Add(FoodToAgentsRatio());
        TimeData.Add(Time.GetTicksMsec());
        TrimData();
        this.cacheTimer.Activate(this.CacheIntervalSeconds);
    }

    private void TrimData()
    {
        if (AgentsCountData.Count > MaxDataPoints)
        {
            AgentsCountData.RemoveRange(0, AgentsCountData.Count - MaxDataPoints);
            AgentsMeanEnergyData.RemoveRange(0, AgentsMeanEnergyData.Count - MaxDataPoints);
            AgentsEnergySumData.RemoveRange(0, AgentsEnergySumData.Count - MaxDataPoints);
            EnvironmentTotalEnergyData.RemoveRange(0, EnvironmentTotalEnergyData.Count - MaxDataPoints);
            FoodToAgentsRatioData.RemoveRange(0, FoodToAgentsRatioData.Count - MaxDataPoints);
            TimeData.RemoveRange(0, TimeData.Count - MaxDataPoints);
        }
    }

    private static int AgentsCount()
    {
        return AgentManager.Instance.GetAgentsCount();
    }

    private static float AgentsMeanEnergy()
    {
        return AgentManager.Instance.GetAgentsEnergySum() / AgentsCount();
    }

    private static float AgentsEnergySum()
    {
        return AgentManager.Instance.GetAgentsEnergySum();
    }

    private static float EnvironmentTotalEnergy()
    {
        float foodEnergy = 0f;
        EntityManager.Instance.FoodBuckets.ForEachEntity(entity => foodEnergy += entity.EnergyNutrition);
        return foodEnergy + AgentsEnergySum();
    }

    private static float FoodToAgentsRatio()
    {
        int foodCount = 0;
        EntityManager.Instance.FoodBuckets.ForEachEntity(_ => foodCount++);
        return (float)foodCount / AgentsCount();
    }
}