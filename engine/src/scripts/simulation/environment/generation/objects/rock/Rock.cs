
public partial class Rock : EnvironmentObject
{
    public override string[] GetInformation()
    {
        return new[]
        {
            $"Position: ({this.GlobalPosition.X:F2}; {this.GlobalPosition.Y:F2})",
        };
    }
}