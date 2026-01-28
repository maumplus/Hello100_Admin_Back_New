using System.Data;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using MySqlConnector;

namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence
{
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

        public IDbConnection CreateDbConnection(DataSource dbType)
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
