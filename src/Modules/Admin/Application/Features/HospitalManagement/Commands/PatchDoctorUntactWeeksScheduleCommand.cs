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
    public record PatchDoctorUntactWeeksScheduleCommandItem
    {
        /// <summary>
        /// 요일순번
        /// </summary>
        public int WeekNum { get; init; }
        /// <summary>
        /// 예약번호
        /// </summary>
        public int Ridx { get; init; }
        /// <summary>
        /// 비대면진료시작(시)
        /// </summary>
        public int UntactStartHour { get; init; }
        /// <summary>
        /// 비대면진료시작(분)
        /// </summary>
        public int UntactStartMinute { get; init; }
        /// <summary>
        /// 비대면진료종료(시)
        /// </summary>
        public int UntactEndHour { get; init; }
        /// <summary>
        /// 비대면진료종료(분)
        /// </summary>
        public int UntactEndMinute { get; init; }
        /// <summary>
        /// 비대면점심시작시간(시)
        /// </summary>
        public int UntactBreakStartHour { get; init; }
        /// <summary>
        /// 비대면점심시작시간(분)
        /// </summary>
        public int UntactBreakStartMinute { get; init; }
        /// <summary>
        /// 비대면점심종료시간(시)
        /// </summary>
        public int UntactBreakEndHour { get; init; }
        /// <summary>
        /// 비대면점심종료시간(분)
        /// </summary>
        public int UntactBreakEndMinute { get; init; }
        /// <summary>
        /// 비대면진료 사용여부
        /// </summary>
        public string UntactUseYn { get; init; } = default!;
    }

    public record PatchDoctorUntactWeeksScheduleCommand : IQuery<Result>
    {
        /// <summary>
        /// 요양기관번호
        /// </summary>
        public string HospNo { get; init; } = default!;
        /// <summary>
        /// 요양기관키
        /// </summary>
        public string HospKey { get; init; } = default!;
        /// <summary>
        /// 의사사번
        /// </summary>
        public string EmplNo { get; init; } = default!;
        /// <summary>
        /// 의사 면허번호
        /// </summary>
        public string DoctNo { get; init; } = default!;
        /// <summary>
        /// 의사명
        /// </summary>
        public string DoctNm { get; init; } = default!;
        /// <summary>
        /// 진료과코드
        /// </summary>
        public string? DeptCd { get; init; }
        /// <summary>
        /// 진료과명
        /// </summary>
        public string? DeptNm { get; init; }
        /// <summary>
        /// 화면 대기인원 표시[0:사용안함, 1:인원수, 2:시간, 3: 인원수, 시간 모두표시]
        /// </summary>
        public int ViewRole { get; init; }
        /// <summary>
        /// 대기 시간표시에 따른 최소시간
        /// </summary>
        public string ViewMinTime { get; init; } = default!;
        /// <summary>
        /// 대기 인원표시에 따른 최소인원
        /// </summary>
        public string ViewMinCnt { get; init; } = default!;
        /// <summary>
        /// 비대면 진료 스케줄 목록
        /// </summary>
        public List<PatchDoctorUntactWeeksScheduleCommandItem> DoctorScheduleList { get; init; } = default!;
    }

    public class PatchDoctorUntactWeeksScheduleCommandHandler : IRequestHandler<PatchDoctorUntactWeeksScheduleCommand, Result>
    {
        private readonly ILogger<PatchDoctorUntactWeeksScheduleCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IDbSessionRunner _db;

        public PatchDoctorUntactWeeksScheduleCommandHandler(
            ILogger<PatchDoctorUntactWeeksScheduleCommandHandler> logger,
            IHospitalManagementRepository hospitalManagementRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _db = db;
        }

        public async Task<Result> Handle(PatchDoctorUntactWeeksScheduleCommand req, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PatchDoctorWeeksScheduleCommand HospNo:{HospNo}", req.HospNo);

            var eghisDoctInfoUntactList = new List<EghisDoctInfoEntity>();

            for (int i = 0; i < req.DoctorScheduleList.Count; i++)
            {
                var doctorSchedule = req.DoctorScheduleList[i];

                var eghisDoctInfoUntactEntity = doctorSchedule.Adapt<EghisDoctInfoEntity>();
                eghisDoctInfoUntactEntity.HospNo = req.HospNo;
                eghisDoctInfoUntactEntity.HospKey = req.HospKey;
                eghisDoctInfoUntactEntity.EmplNo = req.EmplNo;

                eghisDoctInfoUntactList.Add(eghisDoctInfoUntactEntity);
            }

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                await _hospitalManagementRepository.UpdateDoctorInfoUntactScheduleAsync(session, eghisDoctInfoUntactList, token);
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
