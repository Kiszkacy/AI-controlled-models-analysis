

public struct SimulationStatusData
{
    public double TimePassed;
    public bool IsSimulationPaused;

    public SimulationStatusData(double timePassed, bool isSimulationPaused)
    {
        this.TimePassed = timePassed;
        this.IsSimulationPaused = isSimulationPaused;
    }
}