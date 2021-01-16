using System.Data.SqlClient;

namespace TEventStore.Test
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory() : this(null) { }

        public SqlConnectionFactory(string connectionString) => _connectionString = connectionString;

        public virtual string ConnectionString() => _connectionString;
        
        public SqlConnection SqlConnection() => new SqlConnection(ConnectionString());
    }
}
