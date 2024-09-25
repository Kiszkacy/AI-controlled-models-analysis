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

    private PackedScene packedTrainAgent = ResourceLoader.Load<PackedScene>("res://src/scenes/simulation/agent/trainAgent.tscn");
    private PackedScene packedLogicAgent = ResourceLoader.Load<PackedScene>("res://src/scenes/simulation/agent/logicAgent.tscn");

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
        Vector2 position = Vector2.Zero;
        bool isValid = false;
        int tryCount = 0;
        while (tryCount < Config.Get().Environment.SupervisorAgentMaxSpawnTryCount && !isValid)
        {
            tryCount += 1;
            position = new(
                RandomGenerator.Float(this.Environment.Size.X),
                RandomGenerator.Float(this.Environment.Size.Y)
            );

            BiomeType biomeType = EnvironmentGenerationUtil.GetBiomeAt(
                position,
                this.Environment.TemplateData.GenerationSettings.Size,
                this.Environment.TemplateData.GenerationSettings.BiomeChunkSize,
                this.Environment.TemplateData.BiomeData
            );

            if (biomeType == BiomeType.Ocean)
            {
                continue;
            }

            bool failedAgentDistanceCheck = false;
            foreach (var (_, agent_) in AgentManager.Get().Agents)
            {
                if (agent_.GlobalPosition.DistanceTo(position) <= Config.Get().Environment.SupervisorAgentSpawnSafeDistance)
                {
                    failedAgentDistanceCheck = true;
                    break;
                }
            }
            if (failedAgentDistanceCheck)
            {
                continue;
            }

            Vector2I bucketId = EntityManager.Instance.ObjectBuckets.VectorToBucketId(position);
            bool failedObjectDistanceCheck = false;
            foreach (EnvironmentObject object_ in EntityManager.Get().ObjectBuckets.GetEntitiesFrom3x3(bucketId))
            {
                if (object_.GlobalPosition.DistanceTo(position) <= Config.Get().Environment.SupervisorAgentSpawnSafeDistance)
                {
                    failedObjectDistanceCheck = true;
                    break;
                }
            }
            if (failedObjectDistanceCheck)
            {
                continue;
            }

            isValid = true;
        }

        if (isValid)
        {
            Node2D agentInstance = (Node2D)(this.UseLogicAgents ? this.packedLogicAgent : this.packedTrainAgent).Instantiate();
            this.AgentsRootNode.CallDeferred("add_child", agentInstance);
            agentInstance.GlobalPosition = position;
            Agent agent = (Agent)agentInstance;
            agent.Direction = Vector2.FromAngle(RandomGenerator.Float(Mathf.Pi*2.0f));
            AgentManager.Get().RegisterAgent(agent);
        }
    }

    private void SendData()
    {
        List<AgentData> data = new();
        foreach (var (_, agent_) in AgentManager.Get().Agents)
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
            TrainAgent agent = (TrainAgent)AgentManager.Get().Agent(action.Id);
            agent.Action = action;
        }
    }

    public void Reset()
    {

    }
}