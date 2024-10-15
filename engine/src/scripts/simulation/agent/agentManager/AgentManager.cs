
using System.Collections.Generic;
using System.Linq;

public class AgentManager : Singleton<AgentManager>
{
    private readonly System.Collections.Generic.Dictionary<int, Agent> agents = new();
    private int agentIdIterator = 0;

    public Agent Agent(int id) => this.agents[id];
    public System.Collections.Generic.Dictionary<int, Agent> Agents => this.agents;

    public void RegisterAgent(Agent agent)
    {
        this.agents.Add(this.agentIdIterator, agent);
        agent.Id = this.agentIdIterator;
        this.agentIdIterator += 1;
    }

    public void RegisterAgent(int id, Agent agent)
    {
        this.agents.Add(id, agent);
        agent.Id = id;
    }

    public void RemoveAgent(int id)
    {
        this.agents.Remove(id);
    }

    public void RemoveAgent(Agent agent)
    {
        this.agents.Remove(agent.Id);
    }

    public void Reset()
    {
        this.agentIdIterator = 0;
        this.agents.Clear();
    }

    public AgentSaveData[] SaveAgents()
    {
        var agentsList = this.agents.Values.Select(agent => agent.Save()).ToArray();

        return agentsList;
    }
}