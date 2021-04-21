using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using TEventStore.Exceptions;

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
            if (aggregateRecord == null)
                throw new InvalidAggregateRecordException("Aggregate record cannot be null");

            if (eventRecords == null || !eventRecords.Any())
                throw new InvalidEventRecordException("Event records list cannot be null or empty");

            if (eventRecords.Any(x => x == null))
                throw new InvalidEventRecordException("Event record cannot be null");

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

            try
            {
                await using var connection = _sqlConnectionFactory.SqlConnection();
                await connection.ExecuteAsync(StoredEvent.InsertQuery, records).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (e is SqlException && e.Message.Contains("ConcurrencyCheckIndex"))
                    throw new ConcurrencyCheckException(e.Message);

                throw;
            }
        }

        public async Task<IReadOnlyCollection<EventStoreRecord<T>>> GetAsync<T>(string aggregateId)
        {
            if (string.IsNullOrWhiteSpace(aggregateId)) 
                throw new InvalidAggregateIdException("Aggregate Id cannot be null or white space");

            var param = new { AggregateId = aggregateId };

            return await GetAsync<T>(StoredEvent.SelectQuery, param).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<EventStoreRecord<T>>> GetFromSequenceAsync<T>(int sequence, int? take = null)
        {
            if (sequence < 0)
                throw new InvalidSequenceException("Sequence cannot be less the zero");

            var query = take.HasValue? StoredEvent.SelectChunkedWithLimitQuery : StoredEvent.SelectChunkedWithoutLimitQuery;

            var param = new { Sequence = sequence, Take = take };

            return await GetAsync<T>(query, param).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<EventStoreRecord<T>>> GetUntilAsync<T>(string aggregateId, Guid eventId)
        {
            if (string.IsNullOrWhiteSpace(aggregateId))
                throw new InvalidAggregateIdException("Aggregate Id cannot be null or white space");

            if (eventId == Guid.Empty) 
                throw  new InvalidEventIdException("Event Id cannot be empty");

            var param = new { AggregateId = aggregateId, EventId = eventId };

            return await GetAsync<T>(StoredEvent.SelectUntilEventQuery, param).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<EventStoreRecord<T>>> GetUntilAsync<T>(string aggregateId, int sequence)
        {
            if (string.IsNullOrWhiteSpace(aggregateId))
                throw new InvalidAggregateIdException("Aggregate Id cannot be null or white space");

            if (sequence <= 0)
                throw new InvalidSequenceException("Sequence cannot be zero or less the zero");

            var param = new { AggregateId = aggregateId, Sequence = sequence };

            return await GetAsync<T>(StoredEvent.SelectUntilSequenceQuery, param).ConfigureAwait(false);
        }

        public async Task<int> GetLatestSequence()
        {
            await using var connection = _sqlConnectionFactory.SqlConnection();

            return await connection.QueryFirstOrDefaultAsync<int>(StoredEvent.SelectLatestSequenceQuery).ConfigureAwait(false);
        }

        private async Task<IReadOnlyCollection<EventStoreRecord<T>>> GetAsync<T>(string query, object param)
        {
            await using var connection = _sqlConnectionFactory.SqlConnection();

            var storedEvents = (await connection.QueryAsync<StoredEvent>(query, param).ConfigureAwait(false)).ToList().AsReadOnly();

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
