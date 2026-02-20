using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record PostDoctorUntactWeeksReservationCommand : IRequest<Result>
    {
        public string HospNo { get; init; }
        public string EmplNo { get; init; }
        public int WeekNum { get; init; }
        public int UntactRsrvIntervalTime { get; init; }
        public int UntactRsrvIntervalCnt { get; init; }
        public string UntactAvaStartTime { get; init; }
        public string UntactAvaEndTime { get; init; }
        public string UntactAvaUseYn { get; init; }
        public List<PostDoctorUntactWeeksReservationCommandItem> EghisDoctRsrvDetailInfoList { get; init; }
    }

    public record PostDoctorUntactWeeksReservationCommandItem
    {
        public int RsIdx { get; init; }
        public int Ridx { get; init; }
        public string StartTime { get; init; }
        public string EndTime { get; init; }
        public int RsrvCnt { get; init; }
        public int ComCnt { get; init; }
        public string? RegDt { get; init; }
        public string ReceptType { get; init; }
    }

    public class PostDoctorUntactWeeksReservationCommandHandler : IRequestHandler<PostDoctorUntactWeeksReservationCommand, Result>
    {
        private readonly IHospitalManagementRepository _hospitalRepository;
        private readonly ILogger<PostDoctorUntactWeeksReservationCommandHandler> _logger;
        private readonly IDbSessionRunner _db;

        public PostDoctorUntactWeeksReservationCommandHandler(
        IHospitalManagementRepository hospitalRepository,
        ILogger<PostDoctorUntactWeeksReservationCommandHandler> logger,
        IDbSessionRunner db)
        {
            _hospitalRepository = hospitalRepository;
            _logger = logger;
            _db = db;
        }

        public async Task<Result> Handle(PostDoctorUntactWeeksReservationCommand command, CancellationToken ct)
        {
            var eghisDoctRsrvInfoEntity = new EghisDoctRsrvInfoEntity()
            {
                HospNo = command.HospNo,
                EmplNo = command.EmplNo,
                ClinicYmd = string.Empty,
                WeekNum = command.WeekNum,
                UntactRsrvIntervalTime = command.UntactRsrvIntervalTime,
                UntactRsrvIntervalCnt = command.UntactRsrvIntervalCnt,
                UntactAvaStartTime = RemoveColon(command.UntactAvaStartTime),
                UntactAvaEndTime = RemoveColon(command.UntactAvaEndTime),
                UntactAvaUseYn = command.UntactAvaUseYn
            };

            var eghisDoctRsrvDetailInfoEntity = command.EghisDoctRsrvDetailInfoList.Adapt<List<EghisDoctRsrvDetailInfoEntity>>();

            foreach (var item in eghisDoctRsrvDetailInfoEntity)
            {
                item.StartTime = this.RemoveColon(item.StartTime);
                item.EndTime = this.RemoveColon(item.EndTime);
            }

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                await _hospitalRepository.RemoveEghisDoctRsrvAsync(session, eghisDoctRsrvInfoEntity, "NR", token);

                var ridx = await _hospitalRepository.InsertEghisDoctUntactRsrvAsync(session, eghisDoctRsrvInfoEntity, token);

                await _hospitalRepository.InsertEghisDoctRsrvDetailAsync(session, eghisDoctRsrvDetailInfoEntity, ridx, "NR", token);
            },
            ct);

            return Result.Success();
        }

        private string RemoveColon(string? value)
            => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Replace(":", "");
    }
}
