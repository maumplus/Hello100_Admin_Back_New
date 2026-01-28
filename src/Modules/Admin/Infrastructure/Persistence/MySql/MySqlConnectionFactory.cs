using System.Data;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Infrastructure.Configuration.Options;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.MySql
{
    /// <summary>
    /// 특정 벤더(DB) 연결 구현 (MySql)
    /// </summary>
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        #region FILED AREA *********************************************
        private readonly DbConnectionOptions _connectionOptions;
        #endregion

        #region CONSTRUCTOR AREA ***********************************************
        /// <summary>
        /// 
        /// </summary>
        public MySqlConnectionFactory(IOptions<DbConnectionOptions> connectionOptions)
        {
            _connectionOptions = connectionOptions.Value;
        }
        #endregion

        #region ICONNECTIONFACTORY IMPLEMENTS AREA *******************************
        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionOptions.Hello100ConnectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateDbConnection(DataSource dbType)
        {
            string connectionString = dbType switch
            {
                DataSource.Hello100 => _connectionOptions.Hello100ConnectionString,
                _ => throw new ArgumentOutOfRangeException(nameof(dbType))
            };

            return new MySqlConnection(connectionString);
        }
        #endregion
    }
}
