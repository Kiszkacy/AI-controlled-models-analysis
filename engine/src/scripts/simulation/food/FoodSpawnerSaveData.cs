
using Godot;

public struct FoodSpawnerSaveData
{
    public double TimerTime { get; set; }
    public bool TimerIsActive { get; set; }
    public Vector2 Position { get; set; }

    public FoodSpawnerSaveData(double timerTime, bool timerIsActive, Vector2 position)
    {
        this.TimerTime = timerTime;
        this.TimerIsActive = timerIsActive;
        this.Position = position;
    }
}