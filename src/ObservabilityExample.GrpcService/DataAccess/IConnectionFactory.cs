using System.Data.Common;
using Npgsql;

namespace ObservabilityExample.GrpcService.DataAccess;

public interface IConnectionFactory
{
    public Task<DbConnection> CreateConnection(CancellationToken token);
}

public class PostgresConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    public PostgresConnectionFactory(string connectionString)
    {
        _connectionString = !string.IsNullOrEmpty(connectionString)
            ? connectionString
            : throw new ArgumentNullException(nameof(connectionString));
    }

    public Task<DbConnection> CreateConnection(CancellationToken token)
    {
        return Task.FromResult<DbConnection>(new NpgsqlConnection(_connectionString));
    }
}
