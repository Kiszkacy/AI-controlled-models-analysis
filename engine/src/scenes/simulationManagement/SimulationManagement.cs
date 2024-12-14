using Godot;
using System.IO;
using System.Linq;

public partial class SimulationManagement : Control
{
    [ExportGroup("DO NOT EDIT THESE")]
    [Export]
    public Panel SelectedSimulation;

    [Export] public VBoxContainer SimulationContainer;

    public override void _Ready()
    {
        LoadSimulations();
    }
    
    private void LoadSimulations()
    {
        PackedScene simulationButtonScene = GD.Load<PackedScene>("res://src/scenes/simulationManagement/simulationButton.tscn");
        
        string saveDir = Config.Instance.Save.SavePath;
        if (DirAccess.DirExistsAbsolute(saveDir))
        {
            using var dir = DirAccess.Open(saveDir);
            if (dir != null)
            {
                dir.ListDirBegin();
                string fileName = dir.GetNext();
                while (fileName != "")
                {
                    if (!dir.CurrentIsDir() && fileName.EndsWith(".gsave"))
                    {
                        string fullPath = Path.Combine(saveDir, fileName);
                        string pngPath = Path.ChangeExtension(fullPath, ".png");
                        
                        Node simulationButton = simulationButtonScene.Instantiate();

                        if (simulationButton is Control button)
                        {
                            string[] pathParts = Path.GetFileNameWithoutExtension(fileName).Split('-');
                            string simulationName = pathParts[0];
                            
                            var nameLabel = button.FindChild("NameLabel") as Label;
                            if (nameLabel != null)
                                nameLabel.Text = simulationName;

                            string dateTime = string.Join("-", pathParts.Skip(1));
                            var dateTimeLabel = button.FindChild("DateTimeLabel") as Label;
                            if (dateTimeLabel != null)
                                dateTimeLabel.Text = dateTime;
                            
                            if (Godot.FileAccess.FileExists(pngPath))
                            {
                                var textureRect = button.FindChild("TextureRect") as TextureRect;
                                if (textureRect != null)
                                {
                                    var texture = GD.Load<Texture2D>(pngPath);
                                    textureRect.Texture = texture;
                                }
                            }
                            
                            if (button is Button btn)
                            {
                                btn.Pressed += () => OnSimulationButtonPressed(fullPath);
                            }
                        }
                        SimulationContainer.AddChild(simulationButton);
                    }

                    fileName = dir.GetNext();
                }

                dir.ListDirEnd();
            }
        }
    }
    
    private void OnSimulationButtonPressed(string fullPath)
    {
        SelectedSimulation selectedSimulation = SelectedSimulation as SelectedSimulation;
        selectedSimulation.ShowSimulation(fullPath);
    }
}
