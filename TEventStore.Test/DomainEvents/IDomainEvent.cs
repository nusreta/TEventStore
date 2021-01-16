using System;

namespace TEventStore.Test.DomainEvents
{
    public interface IDomainEvent
    {
        public Guid Id { get; }
        public DateTime CreatedAt { get; }
        public string AggregateId { get; }
    }
}
