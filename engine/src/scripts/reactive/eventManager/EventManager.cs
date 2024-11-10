using System.Collections.Generic;

public sealed class EventManager : Singleton<EventManager>
{
    private readonly Dictionary<EventChannel, List<Observable>> eventChannels = new();
    private readonly List<Observable> observers = new();
    private readonly Dictionary<EventChannel, List<IEvent>> delayedChannelEvents = new();
    private readonly List<IEvent> delayedEvents = new();

    private List<Observable> eventChannel(EventChannel eventChannel) => this.GetOrCreateEventList<Observable>(eventChannel, this.eventChannels);
    private List<IEvent> delayedChannelEvent(EventChannel eventChannel) => this.GetOrCreateEventList<IEvent>(eventChannel, this.delayedChannelEvents);

    public void RegisterEvent(IEvent @event, EventChannel? eventChannel = null, bool emitAtTheEndOfFrame = false)
    {
        bool channelEvent = eventChannel.HasValue;
        if (channelEvent && !emitAtTheEndOfFrame)
        {
            this.NotifyObservers(@event, eventChannel.Value);
        }
        else if (channelEvent && emitAtTheEndOfFrame)
        {
            this.delayedChannelEvent(eventChannel.Value).Add(@event);
        }
        else if (!channelEvent && !emitAtTheEndOfFrame)
        {
            this.NotifyObservers(@event);
        }
        else if (!channelEvent && emitAtTheEndOfFrame)
        {
            this.delayedEvents.Add(@event);
        }
    }

    private void NotifyObservers(IEvent @event, EventChannel? eventChannel = null)
    {
        bool channelNotification = eventChannel.HasValue;
        if (channelNotification)
        {
            foreach (Observable observer in this.eventChannel(eventChannel.Value))
            {
                observer.Notify(@event);
            }
        }
        else
        {
            foreach (Observable observer in this.observers)
            {
                observer.Notify(@event);
            }
        }
    }

    public void EmitDelayedEvents()
    {
        foreach (IEvent @event in this.delayedEvents)
        {
            this.NotifyObservers(@event);
        }
        this.delayedEvents.Clear();
        
        foreach ((EventChannel channel, List<IEvent> events) in this.delayedChannelEvents)
        {
            foreach (IEvent @event in events)
            {
                this.NotifyObservers(@event, channel);
            }
        }
        this.delayedEvents.Clear();
    }

    public void Subscribe(Observable observer, EventChannel? eventChannel = null)
    {
        bool channelSubscription = eventChannel.HasValue;
        if (channelSubscription && this.eventChannel(eventChannel.Value).Contains(observer))
        {
            return;
        }
        if (this.observers.Contains(observer))
        {
            return;
        }

        if (channelSubscription)
        {
            this.eventChannel(eventChannel.Value).Add(observer);
        }
        else
        {
            this.observers.Add(observer);
        }
    }

    public void Unsubscribe(Observable observer, EventChannel? eventChannel = null)
    {
        bool channelUnsubscription = eventChannel.HasValue;
        if (channelUnsubscription && !this.eventChannel(eventChannel.Value).Contains(observer))
        {
            return;
        }
        if (!this.observers.Contains(observer))
        {
            return;
        }

        if (channelUnsubscription)
        {
            this.eventChannel(eventChannel.Value).Remove(observer);
        }
        else
        {
            this.observers.Remove(observer);
        }
    }
    
    private List<T> GetOrCreateEventList<T>(EventChannel channel, Dictionary<EventChannel, List<T>> dictionary)
    {
        if (!dictionary.TryGetValue(channel, out var list))
        {
            list = new List<T>();
            dictionary[channel] = list;
        }
        return list;
    }

    private EventManager()
    {

    }
}