using MediatR;

namespace Hello100Admin.BuildingBlocks.Common.Application;

/// <summary>
/// CQRS 커맨드 인터페이스
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// 결과를 반환하는 CQRS 커맨드 인터페이스
/// </summary>
public interface ICommand<TResponse> : IRequest<TResponse>
{
}
