using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using System.Data;
using MySqlConnector;

namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence;
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}