using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TEventStore.Test.DomainEvents;
using Xunit;

namespace TEventStore.Test
{
    public class EventStoreRepositoryTest : SqlBaseTest
    {
        private readonly EventStoreRepository _eventStoreRepository;

        public EventStoreRepositoryTest()
        {
            _eventStoreRepository = new EventStoreRepository(ConnectionFactory);
        }


        [Fact]
        public async Task InsertAsync_GetAsync_SingleAggregate()
        {
            // Given
            const string booId = "001";
            var booCreated = new BooCreated(booId, 100M, false);

            // When
            var aggregateRecord = new AggregateRecord(booId, "Boo", 0);
            var eventRecords = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(booCreated.Id, booCreated.CreatedAt, booCreated)
            };

            await _eventStoreRepository.SaveAsync(aggregateRecord, eventRecords).ConfigureAwait(false);

            // Then
            var results = await _eventStoreRepository.GetAsync<DomainEvent>(booId).ConfigureAwait(false);

            Assert.Equal(1, results.Count);
            Assert.Equal(booCreated.GetType(), results.Single(x => x.AggregateId == booId).Event.GetType());
        }

        [Fact]
        public async Task InsertAsync_GetAsync_SingleAggregate_MultipleEvents()
        {
            // Given
            const string booId = "001";
            var booCreated = new BooCreated(booId, 100M, false);
            var booActivated = new BooActivated(booId);

            // When
            var aggregateRecord = new AggregateRecord(booId, "Boo", 0);
            var eventRecords = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(booCreated.Id, booCreated.CreatedAt, booCreated),
                new EventRecord<DomainEvent>(booActivated.Id, booActivated.CreatedAt, booActivated)
            };

            await _eventStoreRepository.SaveAsync(aggregateRecord, eventRecords).ConfigureAwait(false);

            // Then
            var results = await _eventStoreRepository.GetAsync<DomainEvent>(booId).ConfigureAwait(false);

            Assert.Equal(2, results.Count);
            Assert.Equal(booCreated.GetType(), results.Single(x => x.Version == 1).Event.GetType());
            Assert.Equal(booActivated.GetType(), results.Single(x => x.Version == 2).Event.GetType());
        }


        [Fact]
        public async Task InsertAsync_GetAsync_MultipleAggregates()
        {
            // Given
            const string booId1 = "001";
            var booCreated1 = new BooCreated(booId1, 100M, false);

            const string booId2 = "002";
            var booCreated2 = new BooCreated(booId2, 200M, true);

            // When
            var aggregateRecordBoo1 = new AggregateRecord(booId1, "Boo", 0);
            var eventRecordsBoo1 = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(booCreated1.Id, booCreated1.CreatedAt, booCreated1)
            };

            await _eventStoreRepository.SaveAsync(aggregateRecordBoo1, eventRecordsBoo1).ConfigureAwait(false);

            var aggregateRecordBoo2 = new AggregateRecord(booId2, "Boo", 1);
            var eventRecordsBoo2 = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(booCreated2.Id, booCreated2.CreatedAt, booCreated2)
            };

            await _eventStoreRepository.SaveAsync(aggregateRecordBoo2, eventRecordsBoo2).ConfigureAwait(false);

            // Then
            var resultsBoo1 = await _eventStoreRepository.GetAsync<DomainEvent>(booId1).ConfigureAwait(false);

            Assert.Equal(1, resultsBoo1.Count);
            Assert.Equal(booCreated1.GetType(), resultsBoo1.Single(x => x.AggregateId == booId1).Event.GetType());

            var resultsBoo2 = await _eventStoreRepository.GetAsync<DomainEvent>(booId2).ConfigureAwait(false);

            Assert.Equal(1, resultsBoo2.Count);
            Assert.Equal(booCreated2.GetType(), resultsBoo2.Single(x => x.AggregateId == booId2).Event.GetType());
        }


        [Fact]
        public async Task InsertAsync_GetAsync_MultipleAggregateTypes()
        {
            // Given
            const string booId = "001";
            var booCreated = new BooCreated(booId, 100M, false);

            const string fooId = "100";
            var fooRegistered = new FooRegistered(fooId, "testing foo");

            // When
            var aggregateRecordBoo = new AggregateRecord(booId, "Boo", 0);
            var eventRecordsBoo = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(booCreated.Id, booCreated.CreatedAt, booCreated)
            };

            await _eventStoreRepository.SaveAsync(aggregateRecordBoo, eventRecordsBoo).ConfigureAwait(false);

            var aggregateRecordFoo = new AggregateRecord(fooId, "Foo", 0);
            var eventRecordsFoo = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(fooRegistered.Id, fooRegistered.CreatedAt, fooRegistered)
            };

            await _eventStoreRepository.SaveAsync(aggregateRecordFoo, eventRecordsFoo).ConfigureAwait(false);

            // Then
            var resultsBoo = await _eventStoreRepository.GetAsync<DomainEvent>(booId).ConfigureAwait(false);

            Assert.Equal(1, resultsBoo.Count);
            Assert.Equal(booCreated.GetType(), resultsBoo.Single(x => x.AggregateId == booId).Event.GetType());

            var resultsFoo = await _eventStoreRepository.GetAsync<DomainEvent>(fooId).ConfigureAwait(false);

            Assert.Equal(1, resultsFoo.Count);
            Assert.Equal(fooRegistered.GetType(), resultsFoo.Single(x => x.AggregateId == fooId).Event.GetType());
        }

        [Fact]
        public async Task PropertiesCheck()
        {
            // Given
            const string booId = "001";
            var booCreated = new BooCreated(booId, 100M, false);

            var aggregateRecord = new AggregateRecord(booId, "Boo", 0);
            var eventRecords = new List<EventRecord<DomainEvent>>
                { new EventRecord<DomainEvent>(booCreated.Id, booCreated.CreatedAt, booCreated) }.AsReadOnly();

            await _eventStoreRepository.SaveAsync(aggregateRecord, eventRecords).ConfigureAwait(false);

            // When
            var results = await _eventStoreRepository.GetAsync<DomainEvent>(booId).ConfigureAwait(false);

            // Then
            var @event = results.Single(x => x.AggregateId == booId).Event as BooCreated;

            Assert.Equal(booCreated.Id, @event.Id);
            Assert.Equal(booCreated.CreatedAt, @event.CreatedAt);
            Assert.Equal(booCreated.AggregateId, @event.AggregateId);
            Assert.Equal(booCreated.BooAmount, @event.BooAmount);
            Assert.Equal(booCreated.IsBooActive, @event.IsBooActive);

            Assert.Equal(1, results.First().Version);
            Assert.Equal(1, results.First().Sequence);
        }

        [Fact]
        public async Task ConcurrencyCheck()
        {
            // Given
            const int theSameVersion = 3;
            const string booId = "001";
            var booCreated = new BooCreated(booId, 100M, false);

            var aggregateRecord1 = new AggregateRecord(booId, "Boo", theSameVersion);
            var eventRecords1 = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(booCreated.Id, booCreated.CreatedAt, booCreated)
            };

            await _eventStoreRepository.SaveAsync(aggregateRecord1, eventRecords1).ConfigureAwait(false);

            // When
            var booActivated = new BooActivated(booId);

            var aggregateRecord2 = new AggregateRecord(booId, "Boo", theSameVersion);
            var eventRecords2 = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(booActivated.Id, booActivated.CreatedAt, booCreated)
            };

            // Then
            await Assert.ThrowsAsync<SqlException>(() => _eventStoreRepository.SaveAsync(aggregateRecord2, eventRecords2));
        }
    }
}