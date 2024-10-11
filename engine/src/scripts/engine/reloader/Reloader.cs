
using System;

using Godot;
using Godot.Collections;

public class Reloader: Singleton<Reloader>
{
    public bool IsReloading { get; private set; } = false;
    private readonly String saveFilePath = "user://savegame.yaml";

    public void Reload(Node root)
    {
        IsReloading = true;
        SaveAllData(root);
    }

    public void SaveAllData(Node root)
    {
        Environment environment = (Environment)root.GetNode("Environment");
        Dictionary environmentData = environment.SaveEnvironment();
        Dictionary agentsData = AgentManager.Instance.SaveAgents();
    }
}