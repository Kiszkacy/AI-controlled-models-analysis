
using Godot;
using Godot.Collections;

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
    
    public Dictionary SaveAgents()
    {
        var agentsList = new Array();
        
        foreach (Agent agent in agents.Values)
        {
            var agentData = new Dictionary();
            agent.Save(agentData);
            var agentDict = new Dictionary()
            {
                { "id", agent.Id },
                { "data", agentData }
            };
            agentsList.Add(agentDict);
        }
        var agentsData = new Dictionary()
        {
            { "agents", agentsList }
        };

        return agentsData;
    }
}