using System.Data;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core
{
    /// <summary>
    /// 공통 DB 세션
    /// </summary>
    public sealed class DbSession
    {
        #region PROPERTY AREA ************************************************
        /// <summary>
        /// 
        /// </summary>
        public IDbConnection Connection { get; }

        /// <summary>
        /// 
        /// </summary>
        public IDbTransaction? Transaction { get; }
        #endregion

        #region CONSTRUCTOR AREA ********************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public DbSession(IDbConnection connection, IDbTransaction? transaction = null)
        {
            Connection = connection;
            Transaction = transaction;
        }
        #endregion
    }
}
