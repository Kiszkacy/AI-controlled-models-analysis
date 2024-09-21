
using Godot;

public class EntityManager : Singleton<EntityManager>
{
    public EntityLayer<Food> FoodBuckets { get; private set; }

    public void Setup(Vector2 environmentSize)
    {
        this.FoodBuckets = new EntityLayer<Food>(environmentSize);
    }
}