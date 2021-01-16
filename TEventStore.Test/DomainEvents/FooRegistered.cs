using System;
using Newtonsoft.Json;

namespace TEventStore.Test.DomainEvents
{
    public sealed class FooRegistered : DomainEvent
    {
        public string FooDescription { get; }

        public FooRegistered(string aggregateId, string fooDescription) 
            : base(aggregateId) => FooDescription = fooDescription;

        [JsonConstructor]
        public FooRegistered(string aggregateId, Guid id, DateTime createdAt, string fooDescription)
            : base(id, aggregateId, createdAt) => FooDescription = fooDescription;
    }
}
