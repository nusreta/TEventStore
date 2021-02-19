using System;

namespace TEventStore
{
    internal sealed class StoredEvent
    {
        public string AggregateId { get; set; }
        public string Aggregate { get; set; }
        public int Version { get; set; }
        public int Sequence { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Payload { get; set; }

        public static string InsertQuery =
            @"INSERT INTO [dbo].[EventStore] 
                ([Id], [Name], [AggregateId], [Aggregate], [Version], [CreatedAt], [Payload]) 
                    VALUES (@Id, @Name, @AggregateId, @Aggregate, @Version, @CreatedAt, @Payload);";

        public static string SelectQuery =
            @"SELECT [Id], [AggregateId], [Version], [CreatedAt], [Payload], [Sequence] 
                FROM [dbo].[EventStore] WHERE [AggregateId] = @AggregateId";

        public static string SelectChunkedWithoutLimitQuery =
            @"SELECT [Id], [AggregateId], [Version], [CreatedAt], [Payload], [Sequence] 
                FROM [dbo].[EventStore] 
                ORDER BY [Sequence]
                OFFSET @Skip ROWS";

        public static string SelectChunkedWithLimitQuery =
            @"SELECT [Id], [AggregateId], [Version], [CreatedAt], [Payload], [Sequence] 
                FROM [dbo].[EventStore] 
                ORDER BY [Sequence]
                OFFSET @Skip ROWS
                FETCH NEXT @Take ROWS ONLY";
    }
}
