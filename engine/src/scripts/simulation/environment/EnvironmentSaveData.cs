
using System.Collections.Generic;

using Godot;

public struct EnvironmentSaveData
{
    public EnvironmentTemplate EnvironmentTemplate { get; set; }
    public AgentSaveData[] AllAgentsData { get; set; }
    public Vector2 CameraPosition { get; set; }
    public Vector2 CameraZoom { get; set; }

    public EnvironmentSaveData(EnvironmentTemplate environmentTemplate, AgentSaveData[] allAgentsData, Vector2 cameraPosition,
        Vector2 cameraZoom)
    {
        this.EnvironmentTemplate = environmentTemplate;
        this.AllAgentsData = allAgentsData;
        this.CameraPosition = cameraPosition;
        this.CameraZoom = cameraZoom;
    }
}