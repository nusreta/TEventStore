using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;

namespace TEventStore
{
    public sealed class EventStoreRepository : IEventStoreRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };

        public EventStoreRepository(ISqlConnectionFactory connectionFactory) => _sqlConnectionFactory = connectionFactory;

        public async Task SaveAsync<T>(AggregateRecord aggregateRecord, IReadOnlyCollection<EventRecord<T>> eventRecords)
        {
            if (aggregateRecord == null || eventRecords == null || !eventRecords.Any()) return;

            var version = aggregateRecord.Version;

            var records = eventRecords.Select(eventRecord => new StoredEvent
            {
                AggregateId = aggregateRecord.Id,
                Aggregate = aggregateRecord.Name,
                Version = ++version,
                CreatedAt = eventRecord.CreatedAt,
                Payload = JsonConvert.SerializeObject(eventRecord.Event, Formatting.Indented, _jsonSerializerSettings),
                Id = eventRecord.Id,
                Name = eventRecord.Event.GetType().Name
            });

            await using var connection = _sqlConnectionFactory.SqlConnection();

            await connection.ExecuteAsync(SqlQueries.InsertEvents, records);
        }

        public async Task<IReadOnlyCollection<EventStoreRecord<T>>> GetAsync<T>(string aggregateId)
        {
            if (aggregateId == null) return new List<EventStoreRecord<T>>();

            await using var connection = _sqlConnectionFactory.SqlConnection();

            var storedEvents = (await connection
                .QueryAsync<StoredEvent>(SqlQueries.SelectEvents, new { AggregateId = aggregateId })).ToList().AsReadOnly();

            if (!storedEvents.Any()) return new List<EventStoreRecord<T>>();

            return storedEvents.Select(@event => new EventStoreRecord<T>
            {
                Event = JsonConvert.DeserializeObject<T>(@event.Payload, _jsonSerializerSettings),
                AggregateId = @event.AggregateId,
                CreatedAt = @event.CreatedAt,
                Id = @event.Id,
                Version = @event.Version,
                Sequence = @event.Sequence
            }).ToList().AsReadOnly();
        }
    }
}
