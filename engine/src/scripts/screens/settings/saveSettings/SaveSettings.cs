
using System.IO;

using Godot;
using Godot.Collections;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public partial class SaveSettings : Control
{
    [Export] public Button ExplorerButton;
    [Export] public FileDialog FileDialogNode;

    private string selectedPath = "user://";
    private string initialPath = "user://";

    private readonly string CONFIG_PATH = "./src/config.yaml";

    public override void _Ready()
    {
        this.ExplorerButton.Pressed += this.OnExplorerButtonPressed;
        this.FileDialogNode.DirSelected += this.OnDirSelected;

        this.FileDialogNode.FileMode = FileDialog.FileModeEnum.OpenDir;
        this.FileDialogNode.Access = FileDialog.AccessEnum.Filesystem;
        this.FileDialogNode.Title = "Select a Directory";
    }

    public bool HasUnsavedChanges()
    {
        return initialPath != selectedPath;
    }

    private void OnExplorerButtonPressed()
    {
        Vector2I size = (Vector2I)(GetViewport().GetVisibleRect().Size * 0.8f);
        this.FileDialogNode.PopupCentered(size);
    }

    private void OnDirSelected(string path)
    {
        selectedPath = path + "/";
        GD.Print("Selected directory: " + path);
    }

    public void ResetToDefault()
    {
        selectedPath = "user://";
    }

    public void ApplySettings()
    {
        this.initialPath = this.selectedPath;
        SaveSettingsToConfig();
    }

    public void SaveSettingsToConfig()
    {
        var yaml = File.ReadAllText(CONFIG_PATH);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var configDict = deserializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(yaml);

        var saveData = new SavePathData(selectedPath);

        configDict["save"] = saveData;

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var updatedYaml = serializer.Serialize(configDict);

        File.WriteAllText(CONFIG_PATH, updatedYaml);

        Config.Instance.Save.SavePath = selectedPath;
    }
}

public class SavePathData
{
    public string SavePath { get; set; }

    public SavePathData(string savePath)
    {
        SavePath = savePath;
    }

    public SavePathData() { }
}