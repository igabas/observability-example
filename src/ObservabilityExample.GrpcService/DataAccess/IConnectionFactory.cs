using System.Data.Common;
using Npgsql;

namespace ObservabilityExample.GrpcService.DataAccess;

public interface IConnectionFactory
{
    public Task<DbConnection> CreateConnection(CancellationToken token);
}

public class PostgresConnectionFactory : IConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }


    public Task<DbConnection> CreateConnection(CancellationToken token)
    {
        var connection = _dataSource.CreateConnection();
        return Task.FromResult<DbConnection>(connection);
    }
}
