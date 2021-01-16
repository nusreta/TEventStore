namespace TEventStore
{
    public sealed class AggregateRecord
    {
        public string Id { get; }
        public string Name { get; }
        public int Version { get; }

        public AggregateRecord(string id, string name, int version) => (Id, Name, Version) = (id, name, version);
    }
}
