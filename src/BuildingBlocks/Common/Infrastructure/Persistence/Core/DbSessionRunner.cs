using System.Data.Common;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core
{
    /// <summary>
    /// 공통 트랜잭션 실행
    /// </summary>
    public class DbSessionRunner : IDbSessionRunner
    {
        private readonly IDbConnectionFactory _connFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connFactory"></param>
        public DbSessionRunner(IDbConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task RunAsync(DataSource dbType, Func<DbSession, Task> action, CancellationToken ct)
            => RunInternalAsync(dbType, action, useTransaction: false, ct);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task RunInTransactionAsync(DataSource dbType, Func<DbSession, Task> action, CancellationToken ct)
            => RunInternalAsync(dbType, action, useTransaction: true, ct);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="action"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        private async Task RunInternalAsync(DataSource dbType, Func<DbSession, Task> action, bool useTransaction, CancellationToken ct)
        {
            await using var conn = (DbConnection)_connFactory.CreateDbConnection(dbType);
            await conn.OpenAsync();

            DbTransaction? tran = null;

            if (useTransaction == true)
                tran = await conn.BeginTransactionAsync();

            var session = new DbSession(conn, tran);

            try
            {
                await action(session);

                if (tran != null)
                    await tran.CommitAsync();
            }
            catch
            {
                if (tran != null)
                    await tran.RollbackAsync();

                throw;
            }
            finally
            {
                tran?.Dispose();
            }
        }
    }
}
