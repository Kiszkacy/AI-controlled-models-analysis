
using System;
using System.IO;

using Godot;
using Godot.Collections;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using FileAccess = Godot.FileAccess;

public class Reloader: Singleton<Reloader>
{
    public bool IsReloading { get; private set; } = false;
    private readonly String saveFilePath = "user://savegame.yaml";

    public void Reload(Node root)
    {
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | WILL RELOAD")
            .End();
        IsReloading = true;
        SaveAllData(root);
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | SAVE COMPLETE")
            .End();
        AgentManager.Instance.Reset();
        root.GetTree().CallDeferred("reload_current_scene");
    }

    public void SaveAllData(Node root)
    {
        Environment environment = (Environment)root.GetNode("Environment");
        EnvironmentTemplate environmentTemplate = environment.SaveEnvironment();
        Godot.Collections.Array agentsData = AgentManager.Instance.SaveAgents();
        var saveData = new EnvironmentSaveData(environmentTemplate, agentsData);
        
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | GOT ALL DATA")
            .End();
        
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        String yamlText = serializer.Serialize(saveData);
        FileAccess file = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Write);
        file.StoreString(yamlText);
        file.Close();

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | SAVED TO YAML")
            .End();
    }
    
    public void LoadAllData(Node root)
    {
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | IN LOAD ALL DATA")
            .End();
        
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
        
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | LOADING AGENTS")
            .End();
        Godot.Collections.Array agentsData = environmentSaveData.AgentsData;
        LoadAgents(agentsData, root);
    }

    private void LoadEnvironment(EnvironmentTemplate environmentTemplate, Node root)
    {
        EntityManager.Instance.Initialize(environmentTemplate.GenerationSettings
            .Size);
        Environment environment = (Environment)root.GetNode("Environment");
        environment.Initialize(environmentTemplate);
    }
    
    private void LoadAgents(Godot.Collections.Array agentsData, Node root)
    {
        Supervisor supervisor = (Supervisor)root.GetNode("Supervisor");
        supervisor.LoadAgents(agentsData);
    }
}