# TEventStore


[![Build Status](https://travis-ci.org/nusreta/TEventStore.svg?branch=main)](https://travis-ci.org/nusreta/TEventStore) [![NuGet Version and Downloads count](https://buildstats.info/nuget/TEventStore)](https://www.nuget.org/packages/TEventStore)

 ``` Dapper v2.0.78 ``` ``` Newtonsoft.Json v11.0.2 ``` ``` System.Data.SqlClient v4.8.2 ```


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

```EventStoreRepository``` implements generic methods ```SaveAsync```, ```GetAsync```, ```GetFromSequenceAsync```, ```GetUntilAsync``` and ```GetLatestSequence```.

```csharp
public interface IEventStoreRepository
{
   Task SaveAsync<T>(AggregateRecord aggregateRecord, IReadOnlyCollection<EventRecord<T>> eventRecords);
   Task<IReadOnlyCollection<EventStoreRecord<T>>> GetAsync<T>(string aggregateId);
   Task<IReadOnlyCollection<EventStoreRecord<T>>> GetFromSequenceAsync<T>(int sequence, int? take = null);
   Task<IReadOnlyCollection<EventStoreRecord<T>>> GetUntilAsync<T>(string aggregateId, Guid eventId);
   Task<IReadOnlyCollection<EventStoreRecord<T>>> GetUntilAsync<T>(string aggregateId, int sequence);
   Task<int> GetLatestSequence();
}
```

## Concurrency check

Unique non clustered index ```ConcurrencyCheckIndex``` does not allow saving two events with the same version for an aggregate root.
On attempt to ```SaveAsync``` throws ```ConcurrencyCheckException```.

## Licence

[Apache-2.0](https://choosealicense.com/licenses/apache-2.0/)

## Releases

### Previous
- v1.0.0 - contains methods for storing and fetching events from event store - ```SaveAsync``` ```GetAsync```
- v1.1.0 - provides additional method ```GetFromSequenceAsync``` that can be used for fetching events from certain sequence with adjustable events chunk size 
- v1.1.1 - resolves a bug related to ```GetFromSequenceAsync``` method
- v1.2.0 - improves error handling with custom exceptions, provides additional methods  ```GetUntilAsync``` ```GetLatestSequence```
- v1.2.1 - resolves a bug with order of stored events when fetching from database (now ordered by sequence)
- v1.3.0 - multi target frameworks ```netcoreapp3.1``` ```netstandard2.0```
- v1.3.1 - downgrade Newtonsoft.Json to v11.0.2
- v1.3.2 - new generic ```GetAsync``` method
- v1.3.3 - ```GetAsync``` method exposed via interface

### Latest
- v1.3.4 - do not throw in case of an empty events list on save

### Planned
- change ```Sequence``` type from ```int``` to ```long```
- handle creating ```EventStore``` table to avoid having to trigger sql script before using the package
- provide configurable ```EventStore``` table's, schema's and certain columns' names
- enable encryption of ```EventStore``` data

 

