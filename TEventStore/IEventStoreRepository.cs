using System.Collections.Generic;
using System.Threading.Tasks;

namespace TEventStore
{
    public interface IEventStoreRepository
    {
        Task SaveAsync<T>(AggregateRecord aggregateRecord, IReadOnlyCollection<EventRecord<T>> eventRecords);
        Task<IReadOnlyCollection<EventStoreRecord<T>>> GetAsync<T>(string aggregateId);
        Task<IReadOnlyCollection<EventStoreRecord<T>>> GetFromSequenceAsync<T>(int sequence, int? take = null);
    }
}
