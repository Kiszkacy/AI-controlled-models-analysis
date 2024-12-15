using System;
using System.IO;
using System.Linq;

using Godot;

public partial class SimulationManagement : Control
{
    [ExportGroup("DO NOT EDIT THESE")]
    [Export]
    public Panel SelectedSimulation;

    [Export] public VBoxContainer SimulationContainer;
    [Export] public Button BackButton;

    public override void _Ready()
    {
        this.BackButton.Pressed += this.OnBackButtonPressed;
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

                            var nameLabel = button.FindChild("Name") as Label;
                            if (nameLabel != null)
                                nameLabel.Text = simulationName;

                            if (pathParts.Length > 1)
                            {
                                string dateTimeString = string.Join("-", pathParts.Skip(1));
                                if (DateTime.TryParseExact(dateTimeString, "yyyy-MM-ddTHH-mm-ss", 
                                        System.Globalization.CultureInfo.InvariantCulture, 
                                        System.Globalization.DateTimeStyles.None, 
                                        out DateTime parsedDateTime))
                                {
                                    var dateTimeLabel = button.FindChild("DateTime") as Label;
                                    if (dateTimeLabel != null)
                                        dateTimeLabel.Text = parsedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }

                            if (Godot.FileAccess.FileExists(pngPath))
                            {
                                var textureRect = button.FindChild("Screenshot") as TextureRect;
                                if (textureRect != null)
                                {
                                    var image = new Image();
                                    Error err = image.Load(pngPath);
                                    if (err == Error.Ok)
                                    {
                                        Vector2 rectSize = textureRect.Size;
                                        image.Resize((int)rectSize.X, (int)rectSize.Y);
                                        var imageTexture = ImageTexture.CreateFromImage(image);
                                        textureRect.Texture = imageTexture;
                                    }
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
    
    private void OnBackButtonPressed()
    {
        this.GetTree().ChangeSceneToFile("res://src/scenes/mainMenu/mainMenu.tscn");
    }
}
