
using Godot.Collections;

public struct EnvironmentSaveData
{
    public EnvironmentTemplate EnvironmentTemplate { get; set; }
    public Array AgentsData { get; set; }
    
    public EnvironmentSaveData(EnvironmentTemplate environmentTemplate, Array agentsData)
    {
        this.EnvironmentTemplate = environmentTemplate;
        this.AgentsData = agentsData;
    }
}