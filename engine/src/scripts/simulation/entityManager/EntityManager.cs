
using System.Collections.Generic;

public class EntityManager : Singleton<EntityManager>
{
    private readonly HashSet<Food> foodSet = new();
    private readonly List<Food> foodList = new();
    private readonly Dictionary<int, Agent> agents = new();

    public List<Food> Food => this.foodList;
    public Agent Agent(int id) => this.agents[id];
    public Dictionary<int, Agent> Agents => this.agents;

    private int agentIdIterator = 0;

    public void RegisterFood(Food food)
    {
        if (this.foodSet.Add(food))
        {
            this.foodList.Add(food);
        }
    }

    public void RemoveFood(Food food)
    {
        if (this.foodSet.Remove(food))
        {
            int index = this.foodList.IndexOf(food);
            if (index >= 0)
            {
                int lastIndex = this.foodList.Count - 1;
                this.foodList[index] = this.foodList[lastIndex];
                this.foodList.RemoveAt(lastIndex);
            }
        }
    }

    public void RegisterAgent(Agent agent)
    {
        this.agents.Add(this.agentIdIterator, agent);
        agent.Id = this.agentIdIterator;
        this.agentIdIterator += 1;
    }

    public void RemoveAgent(int id)
    {
        this.agents.Remove(id);
    }

    public void RemoveAgent(Agent agent)
    {
        this.agents.Remove(agent.Id);
    }
}