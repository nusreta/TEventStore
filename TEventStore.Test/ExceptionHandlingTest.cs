﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TEventStore.Exceptions;
using TEventStore.Test.DomainEvents;
using Xunit;

namespace TEventStore.Test
{
    public class ExceptionHandlingTest
    {
        private readonly EventStoreRepository _eventStoreRepository;

        public ExceptionHandlingTest()
        {
            _eventStoreRepository = new EventStoreRepository(null);
        }

        [Fact]
        public void GivenNullAggregateId_WhenConstructingAggregateRecord_ShouldThrowInvalidAggregateIdException()
        {
            // Given
            string aggregateId = null;
            const string name = "Boo";
            const int version = 0;

            // When + Then
            Assert.Throws<InvalidAggregateIdException>(() => new AggregateRecord(aggregateId, name, version));
        }

        [Fact]
        public void GivenEmptyAggregateId_WhenConstructingAggregateRecord_ShouldThrowInvalidAggregateIdException()
        {
            // Given
            var aggregateId = string.Empty;
            const string name = "Boo";
            const int version = 0;

            // When + Then
            Assert.Throws<InvalidAggregateIdException>(() => new AggregateRecord(aggregateId, name, version));
        }

        [Fact]
        public void GivenNullAggregateName_WhenConstructingAggregateRecord_ShouldThrowInvalidAggregateRecordException()
        {
            // Given
            const string aggregateId = "BooId";
            const string name = null;
            const int version = 0;

            // When + Then
            Assert.Throws<InvalidAggregateRecordException>(() => new AggregateRecord(aggregateId, name, version));
        }

        [Fact]
        public void GivenEmptyAggregateName_WhenConstructingAggregateRecord_ShouldThrowInvalidAggregateRecordException()
        {
            // Given
            const string aggregateId = "BooId";
            var name = string.Empty;
            const int version = 0;

            // When + Then
            Assert.Throws<InvalidAggregateRecordException>(() => new AggregateRecord(aggregateId, name, version));
        }

        [Fact]
        public void GivenVersionLessThenZero_WhenConstructingAggregateRecord_ShouldThrowInvalidAggregateRecordException()
        {
            // Given
            const string aggregateId = "BooId";
            const string name = "Boo";
            const int version = -1;

            // When + Then
            Assert.Throws<InvalidAggregateRecordException>(() => new AggregateRecord(aggregateId, name, version));
        }

        [Fact]
        public void GivenEmptyEventId_WhenConstructingEventRecord_ShouldThrowInvalidEventIdException()
        {
            // Given
            var eventId = Guid.Empty;
            var createdAt = DateTime.Now;
            var @event = new BooCreated("AggregateId", 100M, false);

            // When + Then
            Assert.Throws<InvalidEventIdException>(() => new EventRecord<DomainEvent>(eventId, createdAt, @event));
        }

        [Fact]
        public void GivenDefaultCreatedAt_WhenConstructingEventRecord_ShouldThrowInvalidEventRecordException()
        {
            // Given
            var eventId = Guid.NewGuid();
            var createdAt = (DateTime)default;
            var @event = new BooCreated("AggregateId", 100M, false);

            // When + Then
            Assert.Throws<InvalidEventRecordException>(() => new EventRecord<DomainEvent>(eventId, createdAt, @event));
        }

        [Fact]
        public void GivenNullEvent_WhenConstructingEventRecord_ShouldThrowInvalidEventRecordException()
        {
            // Given
            var eventId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            BooCreated @event = null;

            // When + Then
            Assert.Throws<InvalidEventRecordException>(() => new EventRecord<DomainEvent>(eventId, createdAt, @event));
        }


        [Fact]
        public async Task GivenNullAggregateRecord_WhenSaveAsync_ShouldThrowInvalidAggregateRecordException()
        {
            // Given
            var @event = new BooCreated("AggregateId", 100M, false);
            AggregateRecord aggregateRecord = null;
            var eventRecords = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(Guid.NewGuid(), DateTime.Now, @event)
            };

            // When + Then
            await Assert.ThrowsAsync<InvalidAggregateRecordException > (() =>
                _eventStoreRepository.SaveAsync(aggregateRecord, eventRecords));
        }

        [Fact]
        public async Task GivenAnyNullEventRecord_WhenSaveAsync_ShouldThrowInvalidEventRecordException()
        {
            // Given
            var @event = new BooCreated("AggregateId", 100M, false);
            var aggregateRecord = new AggregateRecord("AggregateId", "AggregateName", 0);
            var eventRecords = new List<EventRecord<DomainEvent>>
            {
                null, 
                new EventRecord<DomainEvent>(Guid.NewGuid(), DateTime.Now, @event)
            };

            // When + Then
            await Assert.ThrowsAsync<InvalidEventRecordException>(() =>
                _eventStoreRepository.SaveAsync(aggregateRecord, eventRecords));
        }

        [Fact]
        public async Task GivenNullSqlConnection_WhenSaveAsync_ShouldThrowNullReferenceException()
        {
            // Given
            var @event = new BooCreated("AggregateId", 100M, false);
            var aggregateRecord = new AggregateRecord("AggregateId", "AggregateName", 0);
            var eventRecords = new List<EventRecord<DomainEvent>>
            {
                new EventRecord<DomainEvent>(Guid.NewGuid(), DateTime.Now, @event)
            };

            // When + Then
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _eventStoreRepository.SaveAsync(aggregateRecord, eventRecords));
        }

        [Fact]
        public async Task GivenNullAggregateId_WhenGetAsync_ShouldThrowInvalidAggregateIdException()
        {
            // Given
            string aggregateId = null;

            // When + Then
            await Assert.ThrowsAsync<InvalidAggregateIdException>(() =>
                _eventStoreRepository.GetAsync<DomainEvent>(aggregateId));
        }

        [Fact]
        public async Task GivenWhiteSpaceAggregateId_WhenGetAsync_ShouldThrowInvalidAggregateIdException()
        {
            // Given
            var aggregateId = string.Empty;

            // When + Then
            await Assert.ThrowsAsync<InvalidAggregateIdException>(() =>
                _eventStoreRepository.GetAsync<DomainEvent>(aggregateId));
        }

        [Fact]
        public async Task GivenSequenceLessThenZero_WhenGetFromSequenceAsync_ShouldThrowInvalidSequenceException()
        {
            // Given
            const int sequence = -1;

            // When + Then
            await Assert.ThrowsAsync<InvalidSequenceException>(() =>
                _eventStoreRepository.GetFromSequenceAsync<DomainEvent>(sequence));
        }

        [Fact]
        public async Task GivenNullAggregateId_WhenGetUntilAsync_ShouldThrowInvalidAggregateIdException()
        {
            // Given
            const int sequence = 2;
            const string aggregateId = null;

            // When + Then
            await Assert.ThrowsAsync<InvalidAggregateIdException>(() =>
                _eventStoreRepository.GetUntilAsync<DomainEvent>(aggregateId, sequence));
        }

        [Fact]
        public async Task GivenEmptyAggregateId_WhenGetUntilAsync_ShouldThrowInvalidAggregateIdException()
        {
            // Given
            const int sequence = 2;
            var aggregateId = string.Empty;

            // When + Then
            await Assert.ThrowsAsync<InvalidAggregateIdException>(() =>
                _eventStoreRepository.GetUntilAsync<DomainEvent>(aggregateId, sequence));
        }

        [Fact]
        public async Task GivenSequenceLessThenZero_WhenGetUntilAsync_ShouldThrowInvalidSequenceException()
        {
            // Given
            const int sequence = -1;
            const string aggregateId = "AggregateId";

            // When + Then
            await Assert.ThrowsAsync<InvalidSequenceException>(() =>
                _eventStoreRepository.GetUntilAsync<DomainEvent>(aggregateId, sequence));
        }

        [Fact]
        public async Task GivenZeroSequence_WhenGetUntilAsync_ShouldThrowInvalidSequenceException()
        {
            // Given
            const int sequence = 0;
            const string aggregateId = "AggregateId";

            // When + Then
            await Assert.ThrowsAsync<InvalidSequenceException>(() =>
                _eventStoreRepository.GetUntilAsync<DomainEvent>(aggregateId, sequence));
        }

        [Fact]
        public async Task GivenEmptyEventId_WhenGetUntilEventAsync_ShouldThrowInvalidEventIdException()
        {
            // Given
            var eventId = Guid.Empty;
            const string aggregateId = "AggregateId";

            // When + Then
            await Assert.ThrowsAsync<InvalidEventIdException>(() =>
                _eventStoreRepository.GetUntilAsync<DomainEvent>(aggregateId, eventId));
        }

        [Fact]
        public async Task GivenNullAggregateId_WhenGetUntilEventAsync_ShouldThrowInvalidAggregateIdException()
        {
            // Given
            var eventId = Guid.NewGuid();
            const string aggregateId = null;

            // When + Then
            await Assert.ThrowsAsync<InvalidAggregateIdException>(() =>
                _eventStoreRepository.GetUntilAsync<DomainEvent>(aggregateId, eventId));
        }

        [Fact]
        public async Task GivenEmptyAggregateId_WhenGetUntilEventAsync_ShouldThrowInvalidAggregateIdException()
        {
            // Given
            var eventId = Guid.NewGuid();
            var aggregateId = string.Empty;

            // When + Then
            await Assert.ThrowsAsync<InvalidAggregateIdException>(() =>
                _eventStoreRepository.GetUntilAsync<DomainEvent>(aggregateId, eventId));
        }
    }
}
