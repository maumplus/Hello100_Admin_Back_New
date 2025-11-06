using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Hello100Admin.BuildingBlocks.Common.Application;

/// <summary>
/// Dapper 기반 트랜잭션 처리를 위한 인터페이스
/// </summary>
public interface ITransactionalRepository
{
    /// <summary>
    /// 트랜잭션 시작 및 커밋/롤백을 위한 래퍼 메서드
    /// </summary>
    Task ExecuteInTransactionAsync(Func<IDbConnection, IDbTransaction, Task> action, CancellationToken cancellationToken = default);
}
