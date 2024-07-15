using System;

public class Singleton<T> where T : Singleton<T>
{
    // Lazy<T> should be thread safe !
    private static readonly Lazy<T> instance = new(() =>
    {
        var createdInstance = Activator.CreateInstance(typeof(T), true);
        return (T)createdInstance;
    });

    public static T Instance => instance.Value;

    public static T Get() => Instance;

    protected Singleton() { }
}