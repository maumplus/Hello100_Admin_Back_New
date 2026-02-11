using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record PatchDoctorUntactWeeksScheduleInfo
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
        public string Message { get; set; }
        public int Hello100Role { get; set; }
        public int Ridx { get; set; }
        public int ViewRole { get; set; }
        public string ViewMinTime { get; set; }
        public string ViewMinCnt { get; set; }
        public string UseYn { get; set; }
        public int UntactStartHour { get; set; }
        public int UntactStartMinute { get; set; }
        public int UntactEndHour { get; set; }
        public int UntactEndMinute { get; set; }
        public int UntactIntervalTime { get; set; }
        public string UntactUseYn { get; set; }
        public int UntactBreakStartHour { get; set; }
        public int UntactBreakStartMinute { get; set; }
        public int UntactBreakEndHour { get; set; }
        public int UntactBreakEndMinute { get; set; }
    }

    public record PatchDoctorUntactWeeksScheduleCommand : IQuery<Result>
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
        public List<PatchDoctorUntactWeeksScheduleInfo> DoctorScheduleList { get; set; }
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

        public async Task<Result> Handle(PatchDoctorUntactWeeksScheduleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PatchDoctorWeeksScheduleCommand HospNo:{HospNo}", request.HospNo);

            var eghisDoctInfoList = new List<EghisDoctInfoEntity>();
            var eghisDoctInfoUntactList = new List<EghisDoctInfoEntity>();

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                for (int i = 0; i < request.DoctorScheduleList.Count; i++)
                {
                    var doctorSchedule = request.DoctorScheduleList[i];

                    Hello100RoleType hello100Role = (Hello100RoleType)doctorSchedule.Hello100Role;

                    int ridx = 0;

                    if ((hello100Role & Hello100RoleType.Rsrv) == 0)
                    {
                        var removeEghisDoctRsrvInfoEntity = new EghisDoctRsrvInfoEntity()
                        {
                            Ridx = doctorSchedule.Ridx
                        };

                        await _hospitalManagementRepository.RemoveEghisDoctRsrvAsync(session, removeEghisDoctRsrvInfoEntity, token);
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

                    var eghisDoctInfoUntactEntity = new EghisDoctInfoEntity()
                    {
                        HospNo = doctorSchedule.HospNo,
                        HospKey = doctorSchedule.HospKey,
                        EmplNo = doctorSchedule.EmplNo,
                        WeekNum = doctorSchedule.WeekNum,
                        UntactStartHour = doctorSchedule.UntactStartHour,
                        UntactStartMinute = doctorSchedule.UntactStartMinute,
                        UntactEndHour = doctorSchedule.UntactEndHour,
                        UntactEndMinute = doctorSchedule.UntactEndMinute,
                        UntactIntervalTime = doctorSchedule.UntactIntervalTime,
                        UntactUseYn = doctorSchedule.UntactUseYn,
                        UntactBreakStartHour = doctorSchedule.UntactBreakStartHour,
                        UntactBreakStartMinute = doctorSchedule.UntactBreakStartMinute,
                        UntactBreakEndHour = doctorSchedule.UntactBreakEndHour,
                        UntactBreakEndMinute = doctorSchedule.UntactBreakEndMinute
                    };

                    eghisDoctInfoUntactList.Add(eghisDoctInfoUntactEntity);
                }

                await _hospitalManagementRepository.UpdateDoctorInfoScheduleAsync(session, eghisDoctInfoList, token);
                await _hospitalManagementRepository.UpdateDoctorInfoUntactScheduleAsync(session, eghisDoctInfoUntactList, token);
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
