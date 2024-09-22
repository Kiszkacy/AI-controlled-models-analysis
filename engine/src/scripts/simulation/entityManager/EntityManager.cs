
using Godot;

public class EntityManager : Singleton<EntityManager>, Initializable
{
    public EntityLayer<Food> FoodBuckets { get; private set; }
    
    private readonly InitializableWrapper initialized = new();
    public bool IsInitialized => this.initialized.IsInitialized;

    public void Initialize(Vector2 environmentSize)
    {
        this.FoodBuckets = new EntityLayer<Food>(environmentSize);
        this.FoodBuckets.Initialize();
        this.initialized.Initialize();
    }
}