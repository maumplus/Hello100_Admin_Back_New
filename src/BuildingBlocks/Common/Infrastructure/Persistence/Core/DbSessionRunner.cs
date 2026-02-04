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
        public Task RunAsync(DataSource dbType, Func<DbSession, CancellationToken, Task> action, CancellationToken ct)
            => RunInternalAsync(dbType, action, useTransaction: false, ct);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task<T> RunAsync<T>(DataSource dbType, Func<DbSession, CancellationToken, Task<T>> action, CancellationToken ct)
            => RunInternalAsync(dbType, action, useTransaction: false, ct);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task RunInTransactionAsync(DataSource dbType, Func<DbSession, CancellationToken, Task> action, CancellationToken ct)
            => RunInternalAsync(dbType, action, useTransaction: true, ct);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="action"></param>
        /// <param name="useTransaction"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task RunInternalAsync(
            DataSource dbType,
            Func<DbSession, CancellationToken, Task> action,
            bool useTransaction,
            CancellationToken ct)
        {
            await using var conn = (DbConnection)_connFactory.CreateDbConnection(dbType);
            await conn.OpenAsync(ct);

            await using var tran = useTransaction ? await conn.BeginTransactionAsync(ct) : null;

            var session = new DbSession(conn, tran);

            try
            {
                await action(session, ct);

                if (tran != null)
                    await tran.CommitAsync(ct);
            }
            catch
            {
                if (tran != null)
                    await tran.RollbackAsync(ct);

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="action"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        private async Task<T> RunInternalAsync<T>(
            DataSource dbType, 
            Func<DbSession, CancellationToken, Task<T>> action, 
            bool useTransaction, 
            CancellationToken ct)
        {
            await using var conn = (DbConnection)_connFactory.CreateDbConnection(dbType);
            await conn.OpenAsync(ct);

            await using var tran = useTransaction ? await conn.BeginTransactionAsync(ct) : null;

            var session = new DbSession(conn, tran);

            try
            {
                var result = await action(session, ct);

                if (tran != null)
                    await tran.CommitAsync(ct);

                return result;
            }
            catch
            {
                if (tran != null)
                    await tran.RollbackAsync();

                throw;
            }
        }
    }
}
