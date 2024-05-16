using Godot;

public static class TestUtility
{
    public static void SimulateProcessNode(Node node, double targetTime)
    {
        double tickTime = 1.0 / 60.0;
        double timePassed = 0.0;
        while (timePassed < targetTime)
        {
            timePassed += tickTime;
            node._Process(tickTime);
        }
    }
}