# TEventStore


[![Build Status](https://travis-ci.org/nusreta/TEventStore.svg?branch=main)](https://travis-ci.org/nusreta/TEventStore) [![NuGet Version and Downloads count](https://buildstats.info/nuget/TEventStore)](https://www.nuget.org/packages/TEventStore)


The package handles any type of domain event. Domain events do not need to inherit specific interface. 
```TEventStore``` serializes and stores a domain event in ```MSSQL``` database as ```JSON``` with additional aggregate root's metadata.
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

```EventStoreRepository``` implements generic methods ```SaveAsync```, ```GetAsync``` and ```GetFromSequenceAsync```.

```csharp
public interface IEventStoreRepository
{
   Task SaveAsync<T>(AggregateRecord aggregateRecord, IReadOnlyCollection<EventRecord<T>> eventRecords);
   Task<IReadOnlyCollection<EventStoreRecord<T>>> GetAsync<T>(string aggregateId);
   Task<IReadOnlyCollection<EventStoreRecord<T>>> GetFromSequenceAsync<T>(int sequence, int? take = null);
}
```

## Concurrency check

Unique non clustered index ```ConcurrencyCheckIndex``` does not allow saving two events with the same version for an aggregate root.

## Licence

[Apache-2.0](https://choosealicense.com/licenses/apache-2.0/)

## Releases

Previous releases:
v1.0.0 - contains methods for storing and fetching events from event store - ```SaveAsync``` ```GetAsync```
v1.1.0 - provides additional method ```GetFromSequenceAsync``` that can be used for fetching events from certain sequence with adjustable events chunk size 

Latest release:
v1.1.1 - resolves a bug related to ```GetFromSequenceAsync``` method

Next release:
v.1.1.2 - will change ```Sequence``` type from ```int``` to ```long```

Planned releases:
v.1.2.0 - will improve error handling with custom exceptions
v.1.3.0 - will handle creating ```EventStore``` table to avoid having to trigger sql script before using the package
v.1.4.0 - will provide configurable ```EventStore``` table's, schema's and certain columns' names
v.1.5.0 - will enable encryption of ```EventStore``` data

 

