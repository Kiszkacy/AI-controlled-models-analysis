
using System;
using System.IO;
using System.Threading.Tasks;

using Godot;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using FileAccess = Godot.FileAccess;

public class Reloader : Singleton<Reloader>, Observable, Initializable
{
    public bool IsReloading { get; set; } = false;
    private String saveDir = Config.Instance.Save.SavePath;
    private String saveFileName = "savegame";
    private String loadPath;
    public bool IsInitialized { get; private set; } = false;

    public void Initialize()
    {
        if (IsInitialized) return;

        EventManager.Get().Subscribe(this, EventChannel.Settings);
        IsInitialized = true;
    }

    public async void Reload(Node root)
    {
        IsReloading = true;
        await SaveAllData(root);
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | SAVE COMPLETE")
            .End();
        root.GetTree().ReloadCurrentScene();
        AgentManager.Get().Reset();
        EntityManager.Get().Reset();
        EnvironmentManager.Get().Reset();
        AgentSightRayCastManager.Get().Reset();
        TestRunner.Get().Reset();
    }

    public void LoadSimulation(Node root, String loadFilePath)
    {
        if (SetLoadPath(loadFilePath))
        {
            IsReloading = true;
            root.GetTree().ReloadCurrentScene();
            AgentManager.Get().Reset();
            EntityManager.Get().Reset();
            EnvironmentManager.Get().Reset();
            AgentSightRayCastManager.Get().Reset();
            TestRunner.Get().Reset();
        }
        else
        {
            NeatPrinter.Start()
                .ColorPrint(ConsoleColor.Red, "[RELOADER]")
                .Print("  | LOAD FAILED")
                .End();
        }
    }

    private async Task SaveAllData(Node root)
    {
        string saveFilePath = GetSaveFilePath();
        string simulationFilePath = saveFilePath + ".gsave";
        string screenshotFilePath = saveFilePath + ".png";
        if (this.IsReloading)
        {
            this.loadPath = simulationFilePath;
        }

        Camera camera = ((Camera)root.GetNode("Camera"));
        await camera.TakeScreenshot(screenshotFilePath);

        Environment environment = (Environment)root.GetNode("Environment");
        EnvironmentTemplate environmentTemplate = environment.SaveEnvironment();

        AgentSaveData[] agentsData = AgentManager.Instance.SaveAgents();

        Vector2 cameraPosition = ((Node2D)root.GetNode("Camera")).GlobalPosition;
        Vector2 cameraZoom = ((Camera2D)root.GetNode("Camera")).Zoom;

        FoodSpawnerSaveData[] foodSpawnersData = EntityManager.Instance.SaveFoodSpawners();
        FoodSaveData[] foodData = EntityManager.Instance.SaveFood();

        SimulationStatusData simulationStatusData = SimulationManager.Instance.Save();

        String saveFilePath = saveFileDir + saveFileName;

        var saveData = new EnvironmentSaveData(environmentTemplate, agentsData, cameraPosition, cameraZoom,
            foodSpawnersData, foodData, simulationStatusData);

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        String yamlText = serializer.Serialize(saveData);
        FileAccess file = FileAccess.Open(simulationFilePath, FileAccess.ModeFlags.Write);
        file.StoreString(yamlText);
        file.Close();
    }

    public void LoadAllData(Node root)
    {
        String saveFilePath = saveFileDir + saveFileName;

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[RELOADER]")
            .Print("  | LOADING ENVIRONMENT")
            .End();
        if (!FileAccess.FileExists(loadPath))
        {
            throw new FileNotFoundException($"File '{loadPath}' does not exist.");
        }

        FileAccess file = FileAccess.Open(loadPath, FileAccess.ModeFlags.Read);
        string yaml = file.GetAsText();
        file.Close();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        EnvironmentSaveData environmentSaveData = deserializer.Deserialize<EnvironmentSaveData>(yaml);

        EnvironmentTemplate environmentTemplate = environmentSaveData.EnvironmentTemplate;
        LoadEnvironment(environmentTemplate, root);

        ((Camera2D)root.GetNode("Camera")).GlobalPosition = environmentSaveData.CameraPosition;
        ((Camera2D)root.GetNode("Camera")).Zoom = environmentSaveData.CameraZoom;

        AgentSaveData[] agentsData = environmentSaveData.AllAgentsData;
        LoadAgents(agentsData, root);

        EntityManager.Instance.LoadFoodSpawnersData(environmentSaveData.FoodSpawnersData, environmentSaveData.FoodData);

        SimulationManager.Instance.Load(environmentSaveData.SimulationStatusData);
    }

    private void LoadEnvironment(EnvironmentTemplate environmentTemplate, Node root)
    {
        EntityManager.Instance.Initialize(environmentTemplate.GenerationSettings.Size);
        Environment environment = (Environment)root.GetNode("Environment");
        environment.Initialize(environmentTemplate);
        EnvironmentManager.Instance.Initialize(environment);
    }

    private void LoadAgents(AgentSaveData[] agentsData, Node root)
    {
        Supervisor supervisor = (Supervisor)root.GetNode("Supervisor");
        supervisor.LoadAgents(agentsData);
    }

    public void SetSaveFileName(String saveFileName)
    {
        this.saveFileName = saveFileName;
    }

    private bool SetLoadPath(String loadFilePath)
    {
        if (FileAccess.FileExists(loadFilePath))
        {
            if (loadFilePath.EndsWith(".gsave", StringComparison.OrdinalIgnoreCase))
            {
                this.loadPath = loadFilePath;
                return true;
            }
        }

        return false;
    }

    private string GetSaveFilePath()
    {
        string timestamp = Time.GetDatetimeStringFromSystem().Replace(":", "-").Replace(" ", "_");
        return Path.Combine(this.saveDir, this.saveFileName + "-" + timestamp);
    }

    public void Notify(IEvent @event)
    {
        if (@event is NotifyEvent settingsEvent)
        {
            string oldSaveDir = this.saveDir;
            string newSaveDir = Config.Instance.Save.SavePath;

            if (oldSaveDir == newSaveDir) return;

            if (!DirAccess.DirExistsAbsolute(newSaveDir))
            {
                DirAccess.MakeDirAbsolute(newSaveDir);
            }

            if (DirAccess.DirExistsAbsolute(oldSaveDir))
            {
                using var dir = DirAccess.Open(oldSaveDir);
                if (dir != null)
                {
                    dir.ListDirBegin();
                    string fileName = dir.GetNext();

                    while (fileName != "")
                    {
                        if (!dir.CurrentIsDir() && fileName.EndsWith(".gsave"))
                        {
                            string baseName = Path.GetFileNameWithoutExtension(fileName);
                            string oldGsavePath = Path.Combine(oldSaveDir, fileName);
                            string newGsavePath = Path.Combine(newSaveDir, fileName);
                            DirAccess.CopyAbsolute(oldGsavePath, newGsavePath);
                            DirAccess.RemoveAbsolute(oldGsavePath);

                            string pngFileName = baseName + ".png";
                            string oldPngPath = Path.Combine(oldSaveDir, pngFileName);
                            string newPngPath = Path.Combine(newSaveDir, pngFileName);

                            if (FileAccess.FileExists(oldPngPath))
                            {
                                DirAccess.CopyAbsolute(oldPngPath, newPngPath);
                                DirAccess.RemoveAbsolute(oldPngPath);
                            }
                        }

                        fileName = dir.GetNext();
                    }

                    dir.ListDirEnd();
                }
            }
            this.saveDir = newSaveDir;
        }
    }
}