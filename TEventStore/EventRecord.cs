using System;

namespace TEventStore
{
    public sealed class EventRecord<T>
    {
        public Guid Id { get; }
        public DateTime CreatedAt { get; }
        public T Event { get; }

        public EventRecord(Guid id, DateTime createdAt, T @event) => (Id, CreatedAt, Event) = (id, createdAt, @event);
    }
}
