using System.Data.Common;
using Npgsql;

namespace ObservabilityExample.GrpcService.DataAccess;

public interface IConnectionFactory
{
    public Task<DbConnection> CreateConnection(CancellationToken token);
}

public class PostgresConnectionFactory(NpgsqlDataSource dataSource) : IConnectionFactory
{
    public Task<DbConnection> CreateConnection(CancellationToken token)
    {
        var connection = dataSource.CreateConnection();
        return Task.FromResult<DbConnection>(connection);
    }
}
