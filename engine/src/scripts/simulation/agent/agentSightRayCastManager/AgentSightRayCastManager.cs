
using System;

using Godot;

public class AgentSightRayCastManager : Singleton<AgentSightRayCastManager>, Initializable
{
    private RayCast2D rayCast = new();
    
    public RayCast2D RayCast => this.IsInitialized ? this.rayCast : throw new Exception("Not initialized.");
    
    private readonly InitializableWrapper initialized = new();
    public bool IsInitialized => this.initialized.IsInitialized;

    public void Initialize(Node parent, bool initializesAtTheSameTickAsEnvironment)
    {
        if (initializesAtTheSameTickAsEnvironment)
        {
            parent.CallDeferred("add_child", this.rayCast);
        }
        else
        {
            parent.AddChild(this.rayCast);
        }

        this.SetupRayCastMask();
        
        this.initialized.Initialize();
    }

    private void SetupRayCastMask()
    {
        this.rayCast.CollisionMask = 4;
    }
}