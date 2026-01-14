using Hello100Admin.BuildingBlocks.Common.Application;
using MediatR;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Commands.DeleteAdminUser;

/// <summary>
/// 관리자 삭제 커맨드 핸들러 (Dapper 기반 트랜잭션 적용)
/// </summary>
public class DeleteAdminUserCommandHandler : IRequestHandler<DeleteAdminUserCommand, Result>
{
    private readonly IAdminUserRepository _repository;
    private readonly IAdminUserStore _store;
    private readonly ILogger<DeleteAdminUserCommandHandler> _logger;

    public DeleteAdminUserCommandHandler(
        IAdminUserRepository repository,
        IAdminUserStore store,
        ILogger<DeleteAdminUserCommandHandler> logger)
    {
        _repository = repository;
        _store = store;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteAdminUserCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing admin user deletion for Uid: {Uid}", command.Uid);

        var adminUser = await _store.GetByIdWithAdminUserAsync(command.Uid, cancellationToken);
        if (adminUser is null)
        {
            _logger.LogWarning("Delete failed: Admin user not found for Uid: {Uid}", command.Uid);
            return Result.Success().WithError(new ErrorInfo(0, "NotFoundUserId", "관리자 계정이 존재하지 않습니다."));
        }

        adminUser.Delete();
        await _repository.SaveAsync(adminUser, cancellationToken); // Soft Delete 반영

        _logger.LogInformation("Admin user deleted successfully for Uid: {Uid}", command.Uid);
        return Result.Success();
    }
}
