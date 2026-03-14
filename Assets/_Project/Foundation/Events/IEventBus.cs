using System;

public interface IEventBus
{
    void Publish<TEvent>(TEvent evt);
    void Subscribe<TEvent>(Action<TEvent> listener);
    void Unsubscribe<TEvent>(Action<TEvent> listener);
}
