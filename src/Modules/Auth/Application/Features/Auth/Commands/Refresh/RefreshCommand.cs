using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Refresh;
using Hello100Admin.Modules.Auth.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Refresh
{
    public record RefreshCommand(string Aid, string? UserAgent, string? IpAddress) : ICommand<Result<RefreshResponse>>;

    public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
    {
        public RefreshCommandValidator()
        {
            RuleFor(x => x.Aid)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("계정 아이디는 필수입니다.");
            RuleFor(x => x.UserAgent)
                    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("유저 에이전트는 필수입니다.");
            RuleFor(x => x.IpAddress)
                    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("IP는 필수입니다.");
        }
    }

    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, Result<RefreshResponse>>
    {
        private readonly ILogger<RefreshCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IAuthRepository _authRepository;
        private readonly IAuthStore _authStore;
        private readonly IConfiguration _config;

        public RefreshCommandHandler(ILogger<RefreshCommandHandler> logger, ITokenService tokenService, IAuthRepository authRepository, IAuthStore authStore, IConfiguration config)
        {
            _logger = logger;
            _tokenService = tokenService;
            _authRepository = authRepository;
            _authStore = authStore;
            _config = config;
        }

        public async Task<Result<RefreshResponse>> Handle(RefreshCommand request, CancellationToken ct)
        {
            var adminInfo = await _authStore.GetAdminByAidAsync(request.Aid);

            if (adminInfo == null)
            {
                return Result.Success<RefreshResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
            }

            var adminEntity = new AdminEntity()
            {
                Aid = adminInfo.Aid,
                AccId = adminInfo.AccId,
                AccPwd = adminInfo.AccPwd,
                Grade = adminInfo.Grade,
                Name = adminInfo.Name,
                DelYn = "N",
                AccountLocked = adminInfo.AccountLocked,
                LoginFailCount = adminInfo.LoginFailCount
            };

            var roles = new[] { GetRoleNameByGrade(adminInfo.Grade) };

            var accessToken = _tokenService.GenerateAccessToken(adminInfo, roles);

            adminEntity.AccessToken = accessToken;

            await _authRepository.UpdateAccessTokenAsync(adminEntity, ct);

            var response = new RefreshResponse
            {
                AccessToken = accessToken
            };

            return Result.Success(response);
        }

        private string GetRoleNameByGrade(string grade) => grade switch
        {
            GlobalConstant.AdminRoles.SuperAdmin => nameof(GlobalConstant.AdminRoles.SuperAdmin),
            GlobalConstant.AdminRoles.HospitalAdmin => nameof(GlobalConstant.AdminRoles.HospitalAdmin),
            GlobalConstant.AdminRoles.GeneralAdmin => nameof(GlobalConstant.AdminRoles.GeneralAdmin),
            _ => nameof(GlobalConstant.AdminRoles.GeneralAdmin)
        };
    }
}
