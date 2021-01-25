using System;
using System.Data.SqlClient;
using Dapper;

namespace TEventStore.Test
{
    public abstract class SqlBaseTest : IDisposable
    {
        public static ISqlConnectionFactory ConnectionFactory;

        private static readonly string Database = $"TmpTestDb{Guid.NewGuid().ToString("n").Substring(0, 8)}";

        private const string MasterConnectionString = @"Server=(localdb)\mssqlLocaldb; Database=master; Trusted_Connection=True;";

        protected readonly string ConnectionString = $@"Server=(localdb)\mssqlLocaldb; Database={Database}; Trusted_Connection=True;";

        protected SqlBaseTest() => CreateDatabase();

        public void Dispose() => DeleteDatabase();

        private void CreateDatabase()
        {
            var createDatabase =
                @$"IF NOT EXISTS(SELECT * FROM sys.databases WHERE Name = '{Database}') 
                    BEGIN CREATE DATABASE {Database} END;";

            using var masterConnection = new SqlConnection(MasterConnectionString);

            masterConnection.Execute(createDatabase);

            using var connection = new SqlConnection(ConnectionString);

            connection.Execute(SqlQueries.CreateEventStoreTable);

            ConnectionFactory = new SqlConnectionFactory(ConnectionString);
        }

        private void DeleteDatabase()
        {
            var dropDatabase =
                $@"USE master; 
                IF EXISTS(SELECT * FROM sys.databases WHERE Name = '{Database}') 
                BEGIN 
                    ALTER DATABASE {Database} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE {Database} 
                END;";

            using var connection = new SqlConnection(ConnectionString);

            connection.Execute(dropDatabase);
        }
    }
}
