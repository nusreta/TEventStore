using System;
using Newtonsoft.Json;

namespace TEventStore.Test.DomainEvents
{
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid Id { get; }

        public DateTime CreatedAt { get; }

        public string AggregateId { get; }

        protected DomainEvent(string aggregateId) =>
            (Id, CreatedAt, AggregateId) = (Guid.NewGuid(), DateTime.Now, aggregateId);

        [JsonConstructor]
        protected DomainEvent(Guid id, string aggregateId, DateTime createdAt) =>
            (Id, AggregateId, CreatedAt) = (id, aggregateId, createdAt);
    }
}
