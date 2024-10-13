
using System.Collections.Generic;
using System.Linq;

using Godot;

public class EntityLayer<T> : Initializable where T : Node2D, Bucketable
{
    private HashSet<T>[][] buckets;
    private Vector2 environmentSize;
    private readonly int bucketSize = Config.Get().Environment.BucketSize;

    private HashSet<T> Bucket(Vector2I bucketId) => this.buckets[bucketId.X][bucketId.Y];

    private readonly InitializableWrapper initialized = new();
    public bool IsInitialized => this.initialized.IsInitialized;

    public void Initialize()
    {
        int bucketCountInARow = EnvironmentGenerationUtil.ChunksInARow(this.environmentSize.X, this.bucketSize);
        int bucketCountInAColumn = EnvironmentGenerationUtil.ChunksInARow(this.environmentSize.Y, this.bucketSize);

        this.buckets = new HashSet<T>[bucketCountInAColumn][];
        for (int i = 0; i < bucketCountInAColumn; i++)
        {
            this.buckets[i] = new HashSet<T>[bucketCountInARow];

            for (int j = 0; j < bucketCountInARow; j++)
            {
                this.buckets[i][j] = new HashSet<T>();
            }
        }

        this.initialized.Initialize();
    }

    public Vector2I VectorToBucketId(Vector2 position) // TODO move this function to global thingy majingy ?
    {
        int positionX = (int)(position.X / this.bucketSize);
        int positionY = (int)(position.Y / this.bucketSize);

        return new Vector2I(positionX, positionY);
    }

    public void RegisterEntity(T entity)
    {
        Vector2I bucketId = this.VectorToBucketId(entity.GlobalPosition);
        entity.BucketId = bucketId;
        this.Bucket(bucketId).Add(entity);
    }

    public void RemoveEntity(T entity)
    {
        this.Bucket(entity.BucketId).Remove(entity);
    }

    public T[] GetEntitiesFrom(Vector2I bucketId)
    {
        return this.Bucket(bucketId).ToArray();
    }

    public T[] GetEntitiesFrom3x3(Vector2I bucketId)
    {
        int rows = this.buckets.Length;
        int columns = this.buckets[0].Length;

        HashSet<T> mergedBucket = new(this.Bucket(bucketId));

        if (bucketId.X - 1 >= 0 && bucketId.Y - 1 >= 0)
        {
            mergedBucket.UnionWith(this.Bucket(new(bucketId.X - 1, bucketId.Y - 1)));
        }
        if (bucketId.Y - 1 >= 0)
        {
            mergedBucket.UnionWith(this.Bucket(new(bucketId.X, bucketId.Y - 1)));
        }
        if (bucketId.X + 1 < columns && bucketId.Y - 1 >= 0)
        {
            mergedBucket.UnionWith(this.Bucket(new(bucketId.X + 1, bucketId.Y - 1)));
        }
        if (bucketId.X - 1 >= 0)
        {
            mergedBucket.UnionWith(this.Bucket(new(bucketId.X - 1, bucketId.Y)));
        }
        if (bucketId.X + 1 < columns)
        {
            mergedBucket.UnionWith(this.Bucket(new(bucketId.X + 1, bucketId.Y)));
        }
        if (bucketId.X - 1 >= 0 && bucketId.Y + 1 < rows)
        {
            mergedBucket.UnionWith(this.Bucket(new(bucketId.X - 1, bucketId.Y + 1)));
        }
        if (bucketId.Y + 1 < rows)
        {
            mergedBucket.UnionWith(this.Bucket(new(bucketId.X, bucketId.Y + 1)));
        }
        if (bucketId.X + 1 < columns && bucketId.Y + 1 < rows)
        {
            mergedBucket.UnionWith(this.Bucket(new(bucketId.X + 1, bucketId.Y + 1)));
        }

        return mergedBucket.ToArray();
    }

    public EntityLayer(Vector2 environmentSize)
    {
        this.environmentSize = environmentSize;
    }

    public void Reset()
    {
        this.buckets = null;
        this.initialized.Reset();
    }
}