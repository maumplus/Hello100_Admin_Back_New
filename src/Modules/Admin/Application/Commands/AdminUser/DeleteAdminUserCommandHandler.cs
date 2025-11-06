using Hello100Admin.BuildingBlocks.Common.Application;
using MediatR;
using Microsoft.Extensions.Logging;

using Hello100Admin.Modules.Admin.Domain.Interfaces;

namespace Hello100Admin.Modules.Admin.Application.Commands.AdminUser;

/// <summary>
/// 관리자 삭제 커맨드 핸들러 (Dapper 기반 트랜잭션 적용)
/// </summary>
public class DeleteAdminUserCommandHandler : IRequestHandler<DeleteAdminUserCommand, Result<string>>
{
    private readonly IAdminUserRepository _repository;
    private readonly ILogger<DeleteAdminUserCommandHandler> _logger;

    public DeleteAdminUserCommandHandler(
        IAdminUserRepository repository,
        ILogger<DeleteAdminUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(DeleteAdminUserCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing admin user deletion for Uid: {Uid}", command.Uid);

        var adminUser = await _repository.GetByIdWithAdminUserAsync(command.Uid, cancellationToken);
        if (adminUser is null)
        {
            _logger.LogWarning("Delete failed: Admin user not found for Uid: {Uid}", command.Uid);
            return Result.Failure<string>("관리자 계정이 존재하지 않습니다.");
        }

        adminUser.Delete();
        await _repository.SaveAsync(adminUser, cancellationToken); // Soft Delete 반영

        _logger.LogInformation("Admin user deleted successfully for Uid: {Uid}", command.Uid);
        return Result.Success<string>("삭제 성공");
    }
}
