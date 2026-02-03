using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Logout;

/// <summary>
/// 로그아웃 커맨드 핸들러
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly ILogger<LogoutCommandHandler> _logger;
    private readonly IAuthRepository _authRepository;
    private readonly IAuthStore _authStore;

    public LogoutCommandHandler(ILogger<LogoutCommandHandler> logger, IAuthRepository authRepository, IAuthStore authStore)
    {
        _logger = logger;
        _authRepository = authRepository;
        _authStore = authStore;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var adminInfo = await _authStore.GetAdminByAidAsync(request.Aid);

        if (adminInfo == null)
        {
            return Result.Success().WithError(GlobalErrorCode.AuthFailed.ToError());
        }

        var admin = new AdminEntity()
        {
            Aid = adminInfo.Aid,
            AccId = adminInfo.AccId,
            AccPwd = adminInfo.AccPwd,
            Grade = adminInfo.Grade,
            Name = adminInfo.Name,
            DelYn = "N",
            AccessToken = null,
            RefreshToken = null
        };

        await _authRepository.UpdateTokensAsync(admin, cancellationToken);
        
        return Result.Success();
    }
}
