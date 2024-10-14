
using System;
using System.IO;

using Godot;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using FileAccess = Godot.FileAccess;

public class Reloader : Singleton<Reloader>
{
    public bool IsReloading { get; private set; } = false;
    private readonly String saveFilePath = "user://savegame.yaml";

    public void Reload(Node root)
    {
        IsReloading = true;
        SaveAllData(root);
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | SAVE COMPLETE")
            .End();
        root.GetTree().ReloadCurrentScene();
        AgentManager.Get().Reset();
        EntityManager.Get().Reset();
        AgentSightRayCastManager.Get().Reset();
    }

    private void SaveAllData(Node root)
    {
        Environment environment = (Environment)root.GetNode("Environment");
        EnvironmentTemplate environmentTemplate = environment.SaveEnvironment();
        AgentSaveData[] agentsData = AgentManager.Instance.SaveAgents();
        Vector2 cameraPosition = ((Node2D)root.GetNode("Camera")).GlobalPosition;
        Vector2 cameraZoom = ((Camera2D)root.GetNode("Camera")).Zoom;
        var saveData = new EnvironmentSaveData(environmentTemplate, agentsData, cameraPosition, cameraZoom);

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        String yamlText = serializer.Serialize(saveData);
        FileAccess file = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Write);
        file.StoreString(yamlText);
        file.Close();
    }

    public void LoadAllData(Node root)
    {
        if (!FileAccess.FileExists(saveFilePath))
        {
            throw new FileNotFoundException($"File '{saveFilePath}' does not exist.");
        }

        FileAccess file = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Read);
        string yaml = file.GetAsText();
        file.Close();

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | LOADED SAVE FILE")
            .End();


        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        EnvironmentSaveData environmentSaveData = deserializer.Deserialize<EnvironmentSaveData>(yaml);

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | LOADING ENVIRONMENT")
            .End();
        EnvironmentTemplate environmentTemplate = environmentSaveData.EnvironmentTemplate;
        LoadEnvironment(environmentTemplate, root);

        ((Camera2D)root.GetNode("Camera")).GlobalPosition = environmentSaveData.CameraPosition;
        ((Camera2D)root.GetNode("Camera")).Zoom = environmentSaveData.CameraZoom;

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | LOADING AGENTS")
            .End();
        AgentSaveData[] agentsData = environmentSaveData.AllAgentsData;
        LoadAgents(agentsData, root);
    }

    private void LoadEnvironment(EnvironmentTemplate environmentTemplate, Node root)
    {
        EntityManager.Instance.Initialize(environmentTemplate.GenerationSettings.Size);
        Environment environment = (Environment)root.GetNode("Environment");
        environment.Initialize(environmentTemplate);
    }

    private void LoadAgents(AgentSaveData[] agentsData, Node root)
    {
        Supervisor supervisor = (Supervisor)root.GetNode("Supervisor");
        supervisor.LoadAgents(agentsData);
    }
}