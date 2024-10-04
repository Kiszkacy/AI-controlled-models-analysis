using System;

[Serializable]
public struct AgentSaveData
{
    public float Energy { get; }
    public float Health { get; }
    public float CurrentRotation { get; }
    public float CurrentAcceleration { get; }
    public string SpriteTexturePath { get; }
    public float DirectionX { get; }
    public float DirectionY { get; }
    public float DirectionAngle { get; }
    public float Speed { get; }
    public int Id { get; }
    public float? ThisFrameScore { get; }
    
    public AgentSaveData(float energy, float health, float currentRotation, float currentAcceleration, string spriteTexturePath, 
        float directionX, float directionY, float directionAngle, float speed, int id, float? thisFrameScore = null)
    {
        this.Energy = energy;
        this.Health = health;
        this.CurrentRotation = currentRotation;
        this.CurrentAcceleration = currentAcceleration;
        this.SpriteTexturePath = spriteTexturePath;
        this.DirectionX = directionX;
        this.DirectionY = directionY;
        this.DirectionAngle = directionAngle;
        this.Speed = speed;
        this.Id = id;
        this.ThisFrameScore = thisFrameScore;
    }
}