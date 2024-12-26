
using System.Linq;

public partial class Bush : EnvironmentObject
{
    public override string[] GetInformation()
    {
        if (this.HasNode("FoodSpawner")  && this.GetNode("FoodSpawner") is FoodSpawner spawner)
        {
            return spawner.GetInformation()
                .Concat(new[]
                {
                    $"Position: ({this.GlobalPosition.X:F2}; {this.GlobalPosition.Y:F2})",
                })
                .ToArray();
        }
        else
        {
            return new[]
            {
                $"Position: ({this.GlobalPosition.X:F2}; {this.GlobalPosition.Y:F2})",
            };
        }
    }
}
