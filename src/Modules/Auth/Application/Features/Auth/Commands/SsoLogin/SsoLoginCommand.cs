using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Application.Common.Extensions;
using Hello100Admin.Modules.Auth.Application.Common.Models;
using Hello100Admin.Modules.Auth.Application.Features.Auth.ReadModels;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Login;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using static Hello100Admin.BuildingBlocks.Common.Definition.Enums.GlobalConstant;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.SsoLogin
{
    public record SsoLoginCommand : IRequest<Result<LoginResponse>>
    {
        public string? ChartType { get; set; }
        public required string Id { get; set; }
        public required string Key { get; set; }
        public string? UserAgent { get; init; } = default!;
        public string? IpAddress { get; init; } = default!;
    }

    public class SsoLoginCommandValidator : AbstractValidator<SsoLoginCommand>
    {
        public SsoLoginCommandValidator()
        {
            RuleFor(x => x.ChartType)
                .Must(x => string.IsNullOrWhiteSpace(x) || new string[] { ChartTypes.EghisChart, ChartTypes .NixChart }.Contains(x)).WithMessage("차트구분이 유효하지 않습니다.");
            RuleFor(x => x.Id)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");
            RuleFor(x => x.Key)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("차트에서 전달받은 key는 필수입니다.");
        }
    }

    public class SsoLoginCommandHandler : IRequestHandler<SsoLoginCommand, Result<LoginResponse>>
    {
        private readonly ILogger<SsoLoginCommandHandler> _logger;
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IAuthStore _authStore;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public SsoLoginCommandHandler(
            ILogger<SsoLoginCommandHandler> logger,
            IAuthRepository authRepository,
            ITokenService tokenService,
            IAuthStore authStore,
            ICryptoService cryptoService,
            IDbSessionRunner db)
        {
            _logger = logger;
            _authRepository = authRepository;
            _tokenService = tokenService;
            _authStore = authStore;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result<LoginResponse>> Handle(SsoLoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling SsoLoginCommand Id:{Id}", request.Id);

            var hash = _cryptoService.Convert36To10(request.Key).ToString();

            var hosp = Convert.ToInt64(hash.Substring(0, hash.Length - 6));
            var time = hash.Replace(hosp.ToString(), string.Empty);

            var date = Convert.ToDateTime(Convert.ToInt64($"{hosp - Convert.ToInt64(request.Id)}{time}").ToString("####-##-## ##:##:##"));

            var allowMinutes = 30;

            // 요청일시부터 30분간 로그인 허용
            if (DateTime.Now.ToKoreaTime() > date.AddMinutes(allowMinutes))
            {
                return Result.Success<LoginResponse>().WithError(GlobalErrorCode.ExpiredAuthenticationInfo.ToError());
            }
            
            var adminInfo = await _authStore.GetAdminByHospNoAsync(request.Id, cancellationToken);

            if (adminInfo == null)
            {
                return Result.Success<LoginResponse>().WithError(GlobalErrorCode.AuthFailed.ToError());
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
            var refreshToken = _tokenService.GenerateRefreshToken(adminInfo.Aid, request.IpAddress);

            adminEntity.AccessToken = accessToken;
            adminEntity.RefreshToken = refreshToken.Token;

            await _authRepository.UpdateTokensAsync(adminEntity, cancellationToken);

            var adminLog = new AdminLogEntity()
            {
                Aid = adminInfo.Aid,
                UserAgent = request.UserAgent!,
                IP = request.IpAddress!
            };

            await _authRepository.InsertAdminLogAsync(adminLog, cancellationToken);

            // 병원관리자 한정 병원정보 조회
            CurrentHospitalInfo? hospInfo = null;

            if (adminInfo.Grade == GlobalConstant.AdminRoles.HospitalAdmin && string.IsNullOrWhiteSpace(adminInfo.HospNo) == false)
            {
                hospInfo = await _db.RunAsync(DataSource.Hello100,
                    (session, token) => _authStore.GetHospitalInfoByHospNoAsync(session, adminInfo.HospNo, token),
                cancellationToken);

                if (hospInfo != null)
                {
                    if (!string.IsNullOrEmpty(request.ChartType) && request.ChartType != hospInfo.ChartType)
                    {
                        return Result.Success<LoginResponse>().WithError(GlobalErrorCode.NotSameChartType.ToError());
                    }
                }
            }

            var config = this.GetMapsterConfig();

            var response = new LoginResponse()
            {
                User = adminInfo.Adapt<UserResponse>(config),
                Hospital = hospInfo.Adapt<HospitalInfo>(),
                Token = new TokenInfo
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = refreshToken.ExpiresAt
                }
            };

            response.User.Email = string.IsNullOrWhiteSpace(adminInfo.Email) == false ? _cryptoService.DecryptWithNoVector(adminInfo.Email, CryptoKeyType.Email) : "";
            response.User.Tel = string.IsNullOrWhiteSpace(adminInfo.Tel) == false ? _cryptoService.DecryptWithNoVector(adminInfo.Tel, CryptoKeyType.Mobile) : "";

            return Result.Success(response);
        }

        private string GetRoleNameByGrade(string grade) => grade switch
        {
            GlobalConstant.AdminRoles.SuperAdmin => nameof(GlobalConstant.AdminRoles.SuperAdmin),
            GlobalConstant.AdminRoles.HospitalAdmin => nameof(GlobalConstant.AdminRoles.HospitalAdmin),
            GlobalConstant.AdminRoles.GeneralAdmin => nameof(GlobalConstant.AdminRoles.GeneralAdmin),
            _ => nameof(GlobalConstant.AdminRoles.GeneralAdmin)
        };

        private TypeAdapterConfig GetMapsterConfig()
        {
            var config = new TypeAdapterConfig();

            config.NewConfig<AdminModel, UserResponse>()
                .Map(d => d.Id, s => s.Aid)
                .Map(d => d.AccountId, s => s.AccId);

            return config;
        }
    }
}
