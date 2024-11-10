
using System.Collections.Generic;

using Godot;

public struct EnvironmentSaveData
{
    public EnvironmentTemplate EnvironmentTemplate { get; set; }
    public AgentSaveData[] AllAgentsData { get; set; }
    public Vector2 CameraPosition { get; set; }
    public Vector2 CameraZoom { get; set; }
    public FoodSpawnerSaveData[] FoodSpawnersData { get; set; }
    public FoodSaveData[] FoodData { get; set; }
    public SimulationStatusData SimulationStatusData { get; set; }
    public EnvironmentSaveData(EnvironmentTemplate environmentTemplate, AgentSaveData[] allAgentsData, Vector2 cameraPosition,
        Vector2 cameraZoom, FoodSpawnerSaveData[] foodSpawnersData, FoodSaveData[] foodData, SimulationStatusData simulationStatusData)
    {
        this.EnvironmentTemplate = environmentTemplate;
        this.AllAgentsData = allAgentsData;
        this.CameraPosition = cameraPosition;
        this.CameraZoom = cameraZoom;
        this.FoodSpawnersData = foodSpawnersData;
        this.FoodData = foodData;
        this.SimulationStatusData = simulationStatusData;
    }
}