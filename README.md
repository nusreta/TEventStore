# TEventStore
// TODO

[![Build Status](https://travis-ci.org/nusreta/TEventStore.svg?branch=main)](https://travis-ci.org/nusreta/TEventStore) [![NuGet Version and Downloads count](https://buildstats.info/nuget/TEventStore)](https://www.nuget.org/packages/TEventStore)


The package handles any type of domain event. Domain events do not need to inherit specific interface. 
```TEventStore``` serializes and stores a domain event in a ```MSSQL``` database as ```JSON``` with additional aggregate root's metadata.
It provides aggregate root versioning and concurrency checking.

## Prerequisites

The package provides methods for storing and fetching events into/from the database.
It requires ```dbo.EventStore``` table to be created. SQL script [EventStore.sql](https://github.com/nusreta/TEventStore/blob/main/EventStore.sql) needs to be executed before usage.
It results in creating ```dbo.EventStore``` table with following columns:

	[Id] UNIQUEIDENTIFIER NOT NULL - event record unique identifier
	[Name] NVARCHAR(100) NOT NULL - name of the event
	[AggregateId] NVARCHAR(100) NOT NULL - aggregate root Id
	[Aggregate] NVARCHAR(100) NOT NULL - name of the aggregate root
	[Version] INT NOT NULL - version of the aggregate root
	[Sequence] INT IDENTITY(1,1) NOT NULL - autoincrement sequence event record number
	[CreatedAt] DATETIME2(7) NOT NULL - date of insertion of the event record
	[Payload] NVARCHAR(MAX) NOT NULL - json containig serialized event


## Usage

```EventStoreRepository``` implements generic methods ```SaveAsync``` and ```GetAsync```.

```csharp
public interface IEventStoreRepository
{
     Task SaveAsync<T>(AggregateRecord aggregateRecord, IReadOnlyCollection<EventRecord<T>> eventRecords);
     Task<IReadOnlyCollection<EventStoreRecord<T>>> GetAsync<T>(string aggregateId);
}
```

## Concurrency check

Unique non clustered index ```ConcurrencyCheckIndex``` does not allow saving two events with the same version for an aggregate root.

## Licence

[Apache-2.0](https://choosealicense.com/licenses/apache-2.0/)

## Next release

- Custom exception needs to be thrown instead of ```SqlException``` in case that concurrency check fails
```csharp
ConcurrencyCheckFailedException
```

- New method for fetching stored events up until certain event
```csharp
Task<IReadOnlyCollection<EventStoreRecord<T>>> GetUntilAsync<T>(string aggregateId, Guid id);
```
