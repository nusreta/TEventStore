using System;

namespace TEventStore
{
    public sealed class StoredEvent
    {
        public string AggregateId { get; set; }
        public string Aggregate { get; set; }
        public int Version { get; set; }
        public int Sequence { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Payload { get; set; }
    }
}
