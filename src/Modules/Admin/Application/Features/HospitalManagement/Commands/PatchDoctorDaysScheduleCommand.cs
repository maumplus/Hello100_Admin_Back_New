using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record PatchDoctorDaysScheduleInfo
    {
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string EmplNo { get; set; }
        public string ClinicYmd { get; set; }
        public string DoctNo { get; set; }
        public string DoctNm { get; set; }
        public string DeptCd { get; set; }
        public string DeptNm { get; set; }
        public int WeekNum { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public int BreakStartHour { get; set; }
        public int BreakStartMinute { get; set; }
        public int BreakEndHour { get; set; }
        public int BreakEndMinute { get; set; }
        public int IntervalTime { get; set; }
        public string? Message { get; set; }
        public int Hello100Role { get; set; }
        public int Ridx { get; set; }
        public int ViewRole { get; set; }
        public string ViewMinTime { get; set; }
        public string ViewMinCnt { get; set; }
        public string UseYn { get; set; }
    }

    public record PatchDoctorDaysScheduleCommand : IQuery<Result>
    {
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string DoctNo { get; set; }
        public string DoctNm { get; set; }
        public string DeptCd { get; set; }
        public string DeptNm { get; set; }
        public int ViewRole { get; set; }
        public string ViewMinTime { get; set; }
        public string ViewMinCnt { get; set; }
        public List<PatchDoctorDaysScheduleInfo> DoctorScheduleList { get; set; }
    }

    public class PatchDoctorDaysScheduleCommandHandler : IRequestHandler<PatchDoctorDaysScheduleCommand, Result>
    {
        private readonly ILogger<PatchDoctorDaysScheduleCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IDbSessionRunner _db;

        public PatchDoctorDaysScheduleCommandHandler(
            ILogger<PatchDoctorDaysScheduleCommandHandler> logger,
            IHospitalManagementRepository hospitalManagementRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _db = db;
        }

        public async Task<Result> Handle(PatchDoctorDaysScheduleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PatchDoctorDaysScheduleCommand HospNo:{HospNo}", request.HospNo);

            var eghisDoctInfoList = new List<EghisDoctInfoEntity>();

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                for (int i = 0; i < request.DoctorScheduleList.Count; i++)
                {
                    var doctorSchedule = request.DoctorScheduleList[i];

                    Hello100RoleType hello100Role = (Hello100RoleType)doctorSchedule.Hello100Role;

                    int ridx = 0;

                    if ((hello100Role & Hello100RoleType.Rsrv) == 0)
                    {
                        await _hospitalManagementRepository.RemoveEghisDoctRsrvAsync(session, doctorSchedule.Ridx, token);
                    }

                    var eghisDoctInfoEntity = new EghisDoctInfoEntity()
                    {
                        HospNo = doctorSchedule.HospNo,
                        HospKey = doctorSchedule.HospKey,
                        EmplNo = doctorSchedule.EmplNo,
                        DoctNo = doctorSchedule.DoctNo,
                        DoctNm = doctorSchedule.DoctNm,
                        DeptCd = doctorSchedule.DeptCd,
                        DeptNm = doctorSchedule.DeptNm,
                        ClinicYmd = doctorSchedule.ClinicYmd,
                        WeekNum = doctorSchedule.WeekNum,
                        StartHour = doctorSchedule.StartHour,
                        StartMinute = doctorSchedule.StartMinute,
                        EndHour = doctorSchedule.EndHour,
                        EndMinute = doctorSchedule.EndMinute,
                        BreakStartHour = doctorSchedule.BreakStartHour,
                        BreakStartMinute = doctorSchedule.BreakStartMinute,
                        BreakEndHour = doctorSchedule.BreakEndHour,
                        BreakEndMinute = doctorSchedule.BreakEndMinute,
                        IntervalTime = doctorSchedule.IntervalTime,
                        Message = doctorSchedule.Message,
                        Hello100Role = doctorSchedule.Hello100Role,
                        Ridx = ridx,
                        ViewRole = doctorSchedule.ViewRole,
                        ViewMinTime = doctorSchedule.ViewMinTime,
                        ViewMinCnt = doctorSchedule.ViewMinCnt,
                        UseYn = doctorSchedule.UseYn
                    };

                    eghisDoctInfoList.Add(eghisDoctInfoEntity);
                }

                await _hospitalManagementRepository.UpdateDoctorInfoScheduleAsync(session, eghisDoctInfoList, token);
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
