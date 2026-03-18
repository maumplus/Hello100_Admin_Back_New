using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record PatchDoctorWeeksScheduleInfo
    {
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string EmplNo { get; set; }
        public string ClinicYmd { get; set; }
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
        public string UseYn { get; set; }
    }

    public record PatchDoctorWeeksScheduleCommand : IQuery<Result>
    {
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string EmplNo { get; set; }
        public List<PatchDoctorWeeksScheduleInfo> DoctorScheduleList { get; set; }
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

        public async Task<Result> Handle(PatchDoctorWeeksScheduleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PatchDoctorWeeksScheduleCommand HospNo:{HospNo}", request.HospNo);

            var doctorList = await _hospitalManagementStore.GetDoctorList(request.HospNo, request.EmplNo, cancellationToken);

            if (doctorList.Count == 0)
            {
                return Result.Success().WithError(AdminErrorCode.NotFoundDoctorInfo.ToError());
            }

            var doctorInfo = doctorList[0];

            var eghisDoctInfoList = new List<EghisDoctInfoEntity>();

            EghisDoctInfoEntity? eghisDoctInfoEntity = null; 

            await _db.RunInTransactionAsync(DataSource.Hello100, async (session, token) =>
            {
                for (int i = 0; i < request.DoctorScheduleList.Count; i++)
                {
                    var doctorSchedule = request.DoctorScheduleList[i];

                    Hello100RoleType hello100Role = (Hello100RoleType)doctorSchedule.Hello100Role;

                    int ridx = 0;

                    if ((hello100Role & Hello100RoleType.Rsrv) == 0)
                    {
                        await _hospitalManagementRepository.RemoveEghisDoctRsrvAsync(session, doctorSchedule.Ridx, "RS", token);
                    }

                    eghisDoctInfoEntity = new EghisDoctInfoEntity()
                    {
                        HospNo = doctorSchedule.HospNo,
                        HospKey = doctorSchedule.HospKey,
                        EmplNo = doctorSchedule.EmplNo,
                        DoctNo = doctorInfo.DoctNo,
                        DoctNm = doctorInfo.DoctNm,
                        DeptCd = doctorInfo.DeptCd,
                        DeptNm = doctorInfo.DeptNm,
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
                        ViewRole = doctorInfo.ViewRole,
                        ViewMinTime = doctorInfo.ViewMinTime,
                        ViewMinCnt = doctorInfo.ViewMinCnt,
                        UseYn = doctorSchedule.UseYn
                    };

                    eghisDoctInfoList.Add(eghisDoctInfoEntity);
                }

                await _hospitalManagementRepository.UpdateDoctorInfoScheduleAsync(session, eghisDoctInfoList, token);

                eghisDoctInfoEntity = new EghisDoctInfoEntity
                {
                    HospNo = request.HospNo,
                    HospKey = request.HospKey,
                    EmplNo = request.EmplNo,
                    DoctNm = doctorInfo.DoctNm,
                    DeptCd = doctorInfo.DeptCd,
                    DeptNm = doctorInfo.DeptNm,
                    ViewRole = doctorInfo.ViewRole,
                    ViewMinCnt = doctorInfo.ViewMinCnt,
                    ViewMinTime = doctorInfo.ViewMinTime
                };

                await _hospitalManagementRepository.UpdateDoctorInfoAsync(session, eghisDoctInfoEntity, token);
            },
            cancellationToken);

            return Result.Success();
        }
    }
}
