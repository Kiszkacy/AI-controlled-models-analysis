using System;
using System.Linq;

using Godot;

public partial class Environment : Node2D, Initializable
{
    public Vector2 Size => this.TemplateData.GenerationSettings.Size;
    public EnvironmentTemplate TemplateData { private set; get; }
    
    private readonly InitializableWrapper initialized = new();
    public bool IsInitialized => this.initialized.IsInitialized;

    public void Initialize(EnvironmentTemplate template)
    {
        this.TemplateData = template;
        this.initialized.Initialize();
    }
}