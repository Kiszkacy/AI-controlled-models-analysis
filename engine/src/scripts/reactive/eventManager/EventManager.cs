using System.Collections.Generic;

public sealed class EventManager : Singleton<EventManager>
{
    private readonly List<Observable> observers = new();

    private readonly List<Event> delayedEvents = new();

    public void RegisterEvent(Event @event, bool emitAtTheEndOfFrame = false)
    {
        if (emitAtTheEndOfFrame)
        {
            delayedEvents.Add(@event);
            return;
        }

        this.NotifyObservers(@event);
    }

    private void NotifyObservers(Event @event)
    {
        foreach (Observable observer in this.observers)
        {
            observer.Notify(@event);
        }
    }

    public void EmitDelayedEvents()
    {
        foreach (Event @event in this.delayedEvents)
        {
            this.NotifyObservers(@event);
        }

        this.delayedEvents.Clear();
    }

    public void Subscribe(Observable observer)
    {
        if (this.observers.Contains(observer)) return;

        this.observers.Add(observer);
    }

    public void Unsubscribe(Observable observer)
    {
        if (!this.observers.Contains(observer)) return;

        this.observers.Remove(observer);
    }

    private EventManager()
    {

    }
}