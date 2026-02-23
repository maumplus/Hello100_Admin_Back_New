using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Commands
{
    /// <summary>
    /// 병원 관리자 매핑 삭제 쿼리
    /// </summary>
    /// <param name="LoginAId">로그인 관리자 ID</param>
    /// <param name="AccPwd">로그인 관리자 비밀번호</param>
    /// <param name="HospitalAId">매핑 삭제하려는 병원 관리자 ID</param>
    public record DeleteHospitalAdminMappingCommand(string LoginAId, string AccPwd, string HospitalAId) : IQuery<Result>;

    public class DeleteHospitalAdminMappingCommandValidator : AbstractValidator<DeleteHospitalAdminMappingCommand>
    {
        public DeleteHospitalAdminMappingCommandValidator()
        {
            RuleFor(x => x.LoginAId).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("로그인 사용자의 ID는 필수입니다.");
            RuleFor(x => x.AccPwd).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("로그인 사용자의 패스워드는 필수입니다.");
            RuleFor(x => x.HospitalAId).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("삭제할 병원 관리자 ID는 필수입니다.");
        }
    }

    public class DeleteHospitalAdminMappingCommandHandler : IRequestHandler<DeleteHospitalAdminMappingCommand, Result>
    {
        private readonly ILogger<DeleteHospitalAdminMappingCommandHandler> _logger;
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly IAdminUserStore _adminUserStore;
        private readonly IDbSessionRunner _db;
        private readonly IHasher _hasher;

        public DeleteHospitalAdminMappingCommandHandler(
            ILogger<DeleteHospitalAdminMappingCommandHandler> logger,
            IAdminUserRepository adminUserRepository,
            IAdminUserStore adminUserStore,
            IDbSessionRunner db,
            IHasher hasher)
        {
            _logger = logger;
            _adminUserRepository = adminUserRepository;
            _adminUserStore = adminUserStore;
            _db = db;
            _hasher = hasher;
        }
        public async Task<Result> Handle(DeleteHospitalAdminMappingCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling DeleteHospitalAdminMappingCommandHandler");

            var loginUserInfo = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _adminUserStore.GetAdminByAidAsync(session, req.LoginAId, token),
            ct);

            if (loginUserInfo == null)
                throw new NotFoundException(GlobalErrorCode.UserNotFound.ToError());

            // 패스워드 검증
            if (!_hasher.VerifyHashedData(loginUserInfo.AccPwd, req.AccPwd, req.LoginAId, _logger))
                return Result.Success().WithError(GlobalErrorCode.PasswordAuthFailed.ToError());

            var hospitalUserInfo = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _adminUserStore.GetAdminByAidAsync(session, req.HospitalAId, token),
            ct);

            if (hospitalUserInfo == null)
                return Result.Success().WithError(AdminErrorCode.NotFoundHospital.ToError());

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _adminUserRepository.DeleteHospitalAdminMappingAsync(session, req.HospitalAId, hospitalUserInfo.HospNo, hospitalUserInfo.HospKey, token),
            ct);

            return Result.Success();
        }
    }
}
