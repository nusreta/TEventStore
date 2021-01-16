namespace TEventStore.Test.DomainEvents
{
    public sealed class BooActivated : DomainEvent
    {
        public BooActivated(string aggregateId) : base(aggregateId) { }
    }
}
