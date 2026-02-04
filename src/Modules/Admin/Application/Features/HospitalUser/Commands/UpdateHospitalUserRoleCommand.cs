using DocumentFormat.OpenXml.Drawing.Charts;
using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Commands
{
    /// <summary>
    /// 병원 사용자 권한 수정 Command
    /// </summary>
    /// <param name="UserId">사용자 ID</param>
    /// <param name="UserRole">사용자 권한</param>
    public record UpdateHospitalUserRoleCommand(string UserId, int UserRole) : IQuery<Result>;

    public class UpdateHospitalUserRoleCommandValidator : AbstractValidator<UpdateHospitalUserRoleCommand>
    {
        public UpdateHospitalUserRoleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("사용자 ID는 필수입니다.");
            RuleFor(x => x.UserRole)
                .InclusiveBetween(0, 1).WithMessage("사용자 권한이 범위를 벗어났습니다.");
        }
    }

    public class UpdateHospitalUserRoleCommandHandler : IRequestHandler<UpdateHospitalUserRoleCommand, Result>
    {
        private readonly ILogger<UpdateHospitalUserRoleCommandHandler> _logger;
        private readonly IHospitalUserRepository _hospitalUserRepository;
        private readonly IDbSessionRunner _db;

        public UpdateHospitalUserRoleCommandHandler(
            IHospitalUserRepository hospitalUserRepository,
            ILogger<UpdateHospitalUserRoleCommandHandler> logger,
            IDbSessionRunner db)
        {
            _hospitalUserRepository = hospitalUserRepository;
            _logger = logger;
            _db = db;
        }

        public async Task<Result> Handle(UpdateHospitalUserRoleCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpdateHospitalUserRoleCommand for UserId: {UserId}", req.UserId);

            var tbUserEntity = new TbUserEntity
            {
                UId = req.UserId,
                UserRole = req.UserRole
            };

            await _db.RunAsync(DataSource.Hello100, 
                (dbSession, token) => _hospitalUserRepository.UpdateHospitalUserRoleAsync(dbSession, tbUserEntity, token),
                ct);

            return Result.Success();
        }
    }
}
