using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    /// <summary>
    /// Hello100 설정 등록/수정 커맨드
    /// </summary>
    public record UpsertHello100SettingCommand : IQuery<Result>
    {
        public string HospNo { get; init; } = default!;
        public string HospKey { get; init; } = default!;
        public int NoticeId { get; init; }
        public int StId { get; init; }
        public int Role { get; init; }
        public int AwaitRole { get; init; }
        public string? ReceptEndTime { get; init; }
        public string? Notice { get; init; }
        public int ExamPushSet { get; init; }
    }

    public class UpsertHello100SettingCommandValidator :AbstractValidator<UpsertHello100SettingCommand>
    {
        public UpsertHello100SettingCommandValidator()
        {
            RuleFor(x => x.HospNo).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");
            RuleFor(x => x.HospKey).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관키는 필수입니다.");
        }
    }

    public class UpsertHello100SettingCommandHandler : IRequestHandler<UpsertHello100SettingCommand, Result>
    {
        private readonly ILogger<UpsertHello100SettingCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IDbSessionRunner _db;

        public UpsertHello100SettingCommandHandler(
            ILogger<UpsertHello100SettingCommandHandler> logger,
            IHospitalManagementRepository hospitalManagementRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _db = db;
        }

        public async Task<Result> Handle(UpsertHello100SettingCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpsertHello100SettingCommand HospNo:{HospNo}", req.HospNo);

            var receptEndTime = !string.IsNullOrEmpty(req.ReceptEndTime) ? Convert.ToDateTime(req.ReceptEndTime).ToString("HHmm") : string.Empty;

            var settingEntity = new TbEghisHospSettingsInfoEntity
            {
                HospKey = req.HospKey,
                StId = req.StId,
                Role = req.Role,
                AwaitRole = req.AwaitRole,
                ReceptEndTime = receptEndTime,
                ExamPushSet = req.ExamPushSet
            };

            var noticeEntity = new TbNoticeEntity
            {
                NotiId = req.NoticeId,
                HospKey = req.HospKey,
                Content = req.Notice ?? ""
            };

            await _db.RunAsync(DataSource.Hello100, 
                (session, token) => _hospitalManagementRepository.UpsertHello100SettingAsync(session, req.HospNo, settingEntity, noticeEntity, token)
            , ct);

            return Result.Success();
        }
    }
}
