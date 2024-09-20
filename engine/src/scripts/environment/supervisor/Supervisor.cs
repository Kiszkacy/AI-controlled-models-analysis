using System;
using System.Collections.Generic;
using System.Text;

using Godot;

using Newtonsoft.Json;

public partial class Supervisor : Node
{
    [Export]
    public Node AgentsRootNode;

    [Export]
    public Environment Environment;

    [Export]
    public int InitialAgentCount = 10;

    [Export]
    public bool UseLogicAgents = false;

    private PackedScene packedTrainAgent = ResourceLoader.Load<PackedScene>("res://src/scenes/environment/trainAgent.tscn");
    private PackedScene packedLogicAgent = ResourceLoader.Load<PackedScene>("res://src/scenes/environment/logicAgent.tscn");

    public override void _Ready()
    {
        for (int i = 0; i < this.InitialAgentCount; i++)
        {
            this.SpawnAgent();
        }

        if (!this.UseLogicAgents)
        {
            NeatPrinter.Start()
                .ColorPrint(ConsoleColor.Blue, "[SUPERVISOR]")
                .Print("  | CONNECTING PIPE")
                .End();
            PipeHandler.Get().Connect();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!this.UseLogicAgents)
        {
            this.SendData();
            this.ReceiveData();
        }
    }

    private void SpawnAgent()
    {
        Node2D agentInstance = (Node2D)(this.UseLogicAgents ? this.packedLogicAgent : this.packedTrainAgent).Instantiate();
        this.AgentsRootNode.AddChild(agentInstance);
        Vector2 spawnOffset = new Vector2(
            RandomGenerator.Float(0, this.Environment.Size.X),
            RandomGenerator.Float(0, this.Environment.Size.Y)
        );
        agentInstance.GlobalPosition = this.Environment.GlobalPosition + spawnOffset;
        Agent agent = (Agent)agentInstance;
        EntityManager.Get().RegisterAgent(agent);
    }

    private void SendData()
    {
        List<AgentData> data = new List<AgentData>();
        foreach (var (_, agent_) in EntityManager.Get().Agents)
        {
            TrainAgent agent = (TrainAgent)agent_;
            data.Add(agent.NormalizedData);
        }

        byte[] rawData = JsonConvert.SerializeObject(data).ToUtf8Buffer();
        PipeHandler.Get().Send(rawData);
    }

    private void ReceiveData()
    {
        byte[] data = PipeHandler.Get().Receive();
        string dataString = Encoding.UTF8.GetString(data);

        if (int.TryParse(dataString, out int code))
        {
            this.HandleCommunicationCode(code);
        }
        else
        {
            List<AgentAction> agentActions = JsonConvert.DeserializeObject<List<AgentAction>>(dataString);
            this.AssignActions(agentActions);
        }
    }

    private void HandleCommunicationCode(int code)
    {
        CommunicationCode communicationCode = (CommunicationCode)code;
        switch (communicationCode)
        {
            case CommunicationCode.ResetEnvironment:
                this.Reset();
                break;
        }
    }

    private void AssignActions(List<AgentAction> actions)
    {
        foreach (AgentAction action in actions)
        {
            TrainAgent agent = (TrainAgent)EntityManager.Get().Agent(action.Id);
            agent.Action = action;
        }
    }

    public void Reset()
    {

    }
}