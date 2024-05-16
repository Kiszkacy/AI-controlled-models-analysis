
using System;
using System.Collections.Generic;

public struct ValueChanged<T>
{
    public T NewValue { get; }
    public T OldValue { get; }
    
    public ValueChanged(T newValue, T oldValue)
    {
        this.NewValue = newValue;
        this.OldValue = oldValue;
    }
}

public class ObservableValue<T>
{
    private T value;
    
    public T Value
    {
        get => this.value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(this.value, value))
            {
                T oldValue = this.value;
                this.value = value;
                ValueChanged<T> valueChanged = new ValueChanged<T>(this.value, oldValue);
                this.OnChange?.Invoke(this, valueChanged);
            }
        }
    }

    public event EventHandler<ValueChanged<T>> OnChange;

    public ObservableValue(T value)
    {
        this.value = value;
    }
}