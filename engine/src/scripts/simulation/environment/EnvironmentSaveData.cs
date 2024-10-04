using System;
using System.Collections.Generic;

[Serializable]
public struct EnvironmentSaveData
{
    public EnvironmentTemplate Template { get; }
    public List<AgentSaveData> Agents { get; }
    
}