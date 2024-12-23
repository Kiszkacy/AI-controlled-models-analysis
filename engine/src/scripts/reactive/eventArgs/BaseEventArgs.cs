
using System;

public class BaseEventArgs<T> : EventArgs
{
    public T Value { get; set; }

    public BaseEventArgs(T value)
    {
        Value = value;
    }
}