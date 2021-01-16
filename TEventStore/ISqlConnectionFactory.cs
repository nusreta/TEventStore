using System.Data.SqlClient;

namespace TEventStore
{
    public interface ISqlConnectionFactory
    {
        SqlConnection SqlConnection();
    }
}
