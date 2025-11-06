namespace Hello100Admin.BuildingBlocks.Common.Application;

/// <summary>
/// CQRS 커맨드 핸들러 인터페이스
/// </summary>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// 결과를 반환하는 CQRS 커맨드 핸들러 인터페이스
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
