using Hello100Admin.BuildingBlocks.Common.Definition.Enums;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core
{
    /// <summary>
    /// 공통 트랜잭션 실행 (추상)
    /// </summary>
    public interface IDbSessionRunner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task RunAsync(DataSource dbType, Func<DbSession, Task> action, CancellationToken ct);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task RunInTransactionAsync(DataSource dbType, Func<DbSession, Task> action, CancellationToken ct);
    }
}
