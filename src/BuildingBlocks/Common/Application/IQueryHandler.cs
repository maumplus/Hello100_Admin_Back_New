namespace Hello100Admin.BuildingBlocks.Common.Application;

/// <summary>
/// CQRS 쿼리 핸들러 인터페이스
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
