using System;
using System.Collections.Generic;

public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, Delegate> _handlers = new();

    public void Publish<TEvent>(TEvent evt)
    {
        if (_handlers.TryGetValue(typeof(TEvent), out Delegate handler))
        {
            ((Action<TEvent>)handler)?.Invoke(evt);
        }
    }

    public void Subscribe<TEvent>(Action<TEvent> listener)
    {
        Type eventType = typeof(TEvent);

        if (_handlers.TryGetValue(eventType, out Delegate handler))
        {
            _handlers[eventType] = Delegate.Combine(handler, listener);
            return;
        }

        _handlers[eventType] = listener;
    }

    public void Unsubscribe<TEvent>(Action<TEvent> listener)
    {
        Type eventType = typeof(TEvent);

        if (!_handlers.TryGetValue(eventType, out Delegate handler))
        {
            return;
        }

        Delegate next = Delegate.Remove(handler, listener);

        if (next == null)
        {
            _handlers.Remove(eventType);
            return;
        }

        _handlers[eventType] = next;
    }
}
