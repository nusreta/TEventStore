using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TEventStore
{
    public interface IEventStoreRepository
    {
        Task SaveAsync<T>(AggregateRecord aggregateRecord, IReadOnlyCollection<EventRecord<T>> eventRecords);
        Task<IReadOnlyCollection<EventStoreRecord<T>>> GetAsync<T>(string aggregateId);
        Task<IReadOnlyCollection<EventStoreRecord<T>>> GetFromSequenceAsync<T>(int sequence, int? take = null);
        Task<IReadOnlyCollection<EventStoreRecord<T>>> GetUntilAsync<T>(string aggregateId, Guid eventId);
        Task<IReadOnlyCollection<EventStoreRecord<T>>> GetUntilAsync<T>(string aggregateId, int sequence);
        Task<int> GetLatestSequence();
    }
}
