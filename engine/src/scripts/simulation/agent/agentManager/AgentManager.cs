
using System.Collections.Generic;

public class AgentManager : Singleton<AgentManager>
{
    private readonly Dictionary<int, Agent> agents = new();
    private int agentIdIterator = 0;

    public Agent Agent(int id) => this.agents[id];
    public Dictionary<int, Agent> Agents => this.agents;

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