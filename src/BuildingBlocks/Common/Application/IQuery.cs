using MediatR;

namespace Hello100Admin.BuildingBlocks.Common.Application;

/// <summary>
/// CQRS 쿼리 인터페이스
/// </summary>
public interface IQuery<TResponse> : IRequest<TResponse>
{
}
