using System.Data;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core
{
    /// <summary>
    /// 공통 DB 연결 (추상)
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateDbConnection(DataSource dbType);
    }
}
