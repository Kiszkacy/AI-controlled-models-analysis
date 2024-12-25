
using System;

public class InitializableWrapper
{
    private bool isInitialized = false;

    public bool IsInitialized => this.isInitialized;

    public void Initialize()
    {
        if (this.isInitialized)
        {
            throw new InvalidOperationException("This object is already initialized!");
        }
        this.isInitialized = true;
    }

    public void Reset()
    {
        this.isInitialized = false;
    }
}
