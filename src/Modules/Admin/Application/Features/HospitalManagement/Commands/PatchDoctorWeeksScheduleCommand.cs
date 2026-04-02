using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record PatchDoctorWeeksScheduleCommandItem
    {
        /// <summary>
        /// 요일순번
        /// </summary>
        public int WeekNum { get; init; }
        /// <summary>
        /// 진료시작시간
        /// </summary>
        public int StartHour { get; init; }
        /// <summary>
        /// 진료시작분
        /// </summary>
        public int StartMinute { get; init; }
        /// <summary>
        /// 진료종료시간
        /// </summary>
        public int EndHour { get; init; }
        /// <summary>
        /// 진료종료분
        /// </summary>
        public int EndMinute { get; init; }
        /// <summary>
        /// 점심시작시간
        /// </summary>
        public int BreakStartHour { get; init; }
        /// <summary>
        /// 점심시작분
        /// </summary>
        public int BreakStartMinute { get; init; }
        /// <summary>
        /// 점심종료시간
        /// </summary>
        public int BreakEndHour { get; init; }
        /// <summary>
        /// 점심종료분
        /// </summary>
        public int BreakEndMinute { get; init; }
        /// <summary>
        /// 환자 진료 시간
        /// </summary>
        public int IntervalTime { get; init; }
        /// <summary>
        /// 부가서비스[1: qr접수 , 2:당일접수, 4:예약, 8:qr 접수마감, 16:당일접수마감]
        /// </summary>
        public int Hello100Role { get; init; }
        /// <summary>
        /// 사용유무
        /// </summary>
        public string UseYn { get; init; } = default!;
        /// <summary>
        /// 예약번호
        /// </summary>
        public int Ridx { get; init; }
    }

    public record PatchDoctorWeeksScheduleCommand : IQuery<Result>
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
        /// 주간 스케줄 목록
        /// </summary>
        public List<PatchDoctorWeeksScheduleCommandItem> DoctorScheduleList { get; init; } = default!;
    }

    public class PatchDoctorWeeksScheduleCommandHandler : IRequestHandler<PatchDoctorWeeksScheduleCommand, Result>
    {
        private readonly ILogger<PatchDoctorWeeksScheduleCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IHospitalManagementStore _hospitalManagementStore;
        private readonly ICryptoService _cryptoService;
        private readonly IDbSessionRunner _db;

        public PatchDoctorWeeksScheduleCommandHandler(
            ILogger<PatchDoctorWeeksScheduleCommandHandler> logger,
            IHospitalManagementRepository hospitalManagementRepository,
            IHospitalManagementStore hospitalManagementStore,
            ICryptoService cryptoService,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _hospitalManagementStore = hospitalManagementStore;
            _cryptoService = cryptoService;
            _db = db;
        }

        public async Task<Result> Handle(PatchDoctorWeeksScheduleCommand req, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PatchDoctorWeeksScheduleCommand HospNo:{HospNo}", req.HospNo);

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                var eghisDoctInfoList = new List<EghisDoctInfoEntity>();

                EghisDoctInfoEntity? eghisDoctInfoEntity = null;

                for (int i = 0; i < req.DoctorScheduleList.Count; i++)
                {
                    var doctorSchedule = req.DoctorScheduleList[i];

                    Hello100RoleType hello100Role = (Hello100RoleType)doctorSchedule.Hello100Role;

                    if ((hello100Role & Hello100RoleType.Rsrv) == 0)
                    {
                        // RS: 진료예약(접수 유형)
                        await _hospitalManagementRepository.RemoveEghisDoctRsrvAsync(session, doctorSchedule.Ridx, "RS", token);
                    }

                    eghisDoctInfoEntity = doctorSchedule.Adapt<EghisDoctInfoEntity>();
                    eghisDoctInfoEntity.ClinicYmd = string.Empty; // 주간스케줄의 경우 진료일자 구분이 없으므로 빈값으로 세팅
                    eghisDoctInfoEntity.HospNo = req.HospNo;
                    eghisDoctInfoEntity.HospKey = req.HospKey;
                    eghisDoctInfoEntity.EmplNo = req.EmplNo;
                    eghisDoctInfoEntity.DoctNo = _cryptoService.EncryptWithNoVector(req.DoctNo);
                    eghisDoctInfoEntity.DoctNm = req.DoctNm;
                    eghisDoctInfoEntity.DeptCd = req.DeptCd;
                    eghisDoctInfoEntity.DeptNm = req.DeptNm;
                    eghisDoctInfoEntity.ViewRole = req.ViewRole;
                    eghisDoctInfoEntity.ViewMinTime = req.ViewMinTime;
                    eghisDoctInfoEntity.ViewMinCnt = req.ViewMinCnt;
                    eghisDoctInfoEntity.Ridx = doctorSchedule.Ridx;

                    eghisDoctInfoList.Add(eghisDoctInfoEntity);
                }

                await _hospitalManagementRepository.UpdateDoctorInfoScheduleAsync(session, eghisDoctInfoList, token);
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
