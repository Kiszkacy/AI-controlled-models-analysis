
using Godot;

public class SimulationManager : Singleton<SimulationManager>, Processable
{
    public bool IsSimulationPaused { get; private set; }
    public double TimePassed { get; private set; } // in seconds

    public void Pause(Node anySimulationNode)
    {
        this.IsSimulationPaused = true;
        anySimulationNode.GetTree().Paused = true;
    }

    public void Resume(Node anySimulationNode)
    {
        this.IsSimulationPaused = false;
        anySimulationNode.GetTree().Paused = false;
    }

    public void Process(double delta)
    {
        if (this.IsSimulationPaused)
        {
            return;
        }

        this.TimePassed += delta;
    }

    public void Load(SimulationStatusData data)
    {
        this.TimePassed = data.TimePassed;
        this.IsSimulationPaused = data.IsSimulationPaused;
    }

    public SimulationStatusData Save()
    {
        return new(this.TimePassed, this.IsSimulationPaused);
    }
}