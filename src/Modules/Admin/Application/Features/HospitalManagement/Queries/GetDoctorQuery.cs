using DocumentFormat.OpenXml.EMMA;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public class GetDoctorQuery : IRequest<Result<GetDoctorResult?>>
    {
        public string HospNo { get; set; } = string.Empty;
        public string EmplNo { get; set; } = string.Empty;
    }

    public class GetDoctorQueryHandler : IRequestHandler<GetDoctorQuery, Result<GetDoctorResult?>>
    {
        private readonly string _adminImageUrl;
        private readonly ICryptoService _cryptoService;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetDoctorQueryHandler> _logger;

        public GetDoctorQueryHandler(IConfiguration config, ICryptoService cryptoService, IHospitalManagementStore hospitalStore, ILogger<GetDoctorQueryHandler> logger)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _cryptoService = cryptoService;
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<GetDoctorResult?>> Handle(GetDoctorQuery query, CancellationToken cancellationToken)
        {
            var doctorScheduleResult = await _hospitalStore.GetDoctorList(query.HospNo, query.EmplNo, cancellationToken);

            DoctorInfoResult? doctorInfo = null;
            List<DoctorScheduleResult> weeksScheduleList = new List<DoctorScheduleResult>();
            List<DoctorScheduleResult> daysScheduleList = new List<DoctorScheduleResult>();
            List<DoctorScheduleResult> untactWeeksScheduleList = new List<DoctorScheduleResult>();

            if (doctorScheduleResult.Count > 0)
            {
                doctorInfo = new DoctorInfoResult()
                {
                    HospNo = doctorScheduleResult[0].HospNo,
                    HospKey = doctorScheduleResult[0].HospKey,
                    EmplNo = doctorScheduleResult[0].EmplNo,
                    DoctNo = _cryptoService.DecryptWithNoVector(doctorScheduleResult[0].DoctNo),
                    DoctNm = doctorScheduleResult[0].DoctNm,
                    DeptCd = doctorScheduleResult[0].DeptCd,
                    DeptNm = doctorScheduleResult[0].DeptNm,
                    EghisDoctInfoMdList = await _hospitalStore.GetEghisDoctInfoMd(query.HospNo, query.EmplNo, cancellationToken),
                    ViewRole = doctorScheduleResult[0].ViewRole,
                    ViewMinCnt = doctorScheduleResult[0].ViewMinCnt,
                    ViewMinTime = doctorScheduleResult[0].ViewMinTime,
                    UntactJoinYn = doctorScheduleResult[0].UntactJoinYn,
                    DoctModifyYn = doctorScheduleResult[0].DoctModifyYn,
                    ImageUrl = $"{_adminImageUrl}{doctorScheduleResult[0].DoctFilePath}"
                };

                //주중스케줄 작성
                for (var i = 0; i < 8; i++)
                {
                    var schedule = doctorScheduleResult.Where(x => x.WeekNum == i + 1 && string.IsNullOrEmpty(x.ClinicYmd) == true).ToList();
                    var weekName = "월요일";

                    switch (i + 1)
                    {
                        case 1:
                            weekName = "월요일";
                            break;
                        case 2:
                            weekName = "화요일";
                            break;
                        case 3:
                            weekName = "수요일";
                            break;
                        case 4:
                            weekName = "목요일";
                            break;
                        case 5:
                            weekName = "금요일";
                            break;
                        case 6:
                            weekName = "토요일";
                            break;
                        case 7:
                            weekName = "일요일";
                            break;
                        case 8:
                            weekName = "공휴일";
                            break;
                    }

                    if (schedule == null || schedule.Count == 0)
                    {
                        weeksScheduleList.Add(new DoctorScheduleResult
                        {
                            HospNo = doctorScheduleResult[0].HospNo,
                            HospKey = doctorScheduleResult[0].HospKey,
                            EmplNo = doctorScheduleResult[0].EmplNo,
                            ClinicYmd = string.Empty,
                            WeekNum = i + 1,
                            WeekNm = weekName,
                            StartHour = doctorScheduleResult[0].StartHour,
                            StartMinute = doctorScheduleResult[0].StartMinute,
                            EndHour = doctorScheduleResult[0].EndHour,
                            EndMinute = doctorScheduleResult[0].EndMinute,
                            BreakStartHour = doctorScheduleResult[0].BreakStartHour,
                            BreakStartMinute = doctorScheduleResult[0].BreakStartMinute,
                            BreakEndHour = doctorScheduleResult[0].BreakEndHour,
                            BreakEndMinute = doctorScheduleResult[0].BreakEndMinute,
                            IntervalTime = doctorScheduleResult[0].IntervalTime,
                            Hello100Role = (int)(Hello100RoleType.QR | Hello100RoleType.Recept),
                            Ridx = 0,
                            UseYn = "N",
                            RsrvCnt = 0,
                            UntactStartHour = doctorScheduleResult[0].UntactStartHour,
                            UntactStartMinute = doctorScheduleResult[0].UntactStartMinute,
                            UntactEndHour = doctorScheduleResult[0].UntactEndHour,
                            UntactEndMinute = doctorScheduleResult[0].UntactEndMinute,
                            UntactIntervalTime = doctorScheduleResult[0].UntactIntervalTime,
                            UntactBreakStartHour = doctorScheduleResult[0].UntactBreakStartHour,
                            UntactBreakEndHour = doctorScheduleResult[0].UntactBreakEndHour,
                            UntactBreakStartMinute = doctorScheduleResult[0].UntactBreakStartMinute,
                            UntactBreakEndMinute = doctorScheduleResult[0].UntactBreakEndMinute,
                            UntactRsrvCnt = doctorScheduleResult[0].UntactRsrvCnt,
                            UntactUseYn = doctorScheduleResult[0].UntactUseYn
                        });

                        untactWeeksScheduleList.Add(new DoctorScheduleResult
                        {
                            HospNo = doctorScheduleResult[0].HospNo,
                            HospKey = doctorScheduleResult[0].HospKey,
                            EmplNo = doctorScheduleResult[0].EmplNo,
                            ClinicYmd = string.Empty,
                            WeekNum = i + 1,
                            WeekNm = weekName,
                            StartHour = doctorScheduleResult[0].StartHour,
                            StartMinute = doctorScheduleResult[0].StartMinute,
                            EndHour = doctorScheduleResult[0].EndHour,
                            EndMinute = doctorScheduleResult[0].EndMinute,
                            BreakStartHour = doctorScheduleResult[0].BreakStartHour,
                            BreakStartMinute = doctorScheduleResult[0].BreakStartMinute,
                            BreakEndHour = doctorScheduleResult[0].BreakEndHour,
                            BreakEndMinute = doctorScheduleResult[0].BreakEndMinute,
                            IntervalTime = doctorScheduleResult[0].IntervalTime,
                            Hello100Role = (int)(Hello100RoleType.QR | Hello100RoleType.Recept),
                            Ridx = 0,
                            UseYn = "N",
                            RsrvCnt = 0,
                            UntactStartHour = doctorScheduleResult[0].UntactStartHour,
                            UntactStartMinute = doctorScheduleResult[0].UntactStartMinute,
                            UntactEndHour = doctorScheduleResult[0].UntactEndHour,
                            UntactEndMinute = doctorScheduleResult[0].UntactEndMinute,
                            UntactIntervalTime = doctorScheduleResult[0].UntactIntervalTime,
                            UntactBreakStartHour = doctorScheduleResult[0].UntactBreakStartHour,
                            UntactBreakEndHour = doctorScheduleResult[0].UntactBreakEndHour,
                            UntactBreakStartMinute = doctorScheduleResult[0].UntactBreakStartMinute,
                            UntactBreakEndMinute = doctorScheduleResult[0].UntactBreakEndMinute,
                            UntactRsrvCnt = doctorScheduleResult[0].UntactRsrvCnt,
                            UntactUseYn = doctorScheduleResult[0].UntactUseYn
                        });
                    }
                    else
                    {
                        weeksScheduleList.Add(new DoctorScheduleResult
                        {
                            HospNo = schedule[0].HospNo,
                            HospKey = schedule[0].HospKey,
                            EmplNo = schedule[0].EmplNo,
                            ClinicYmd = schedule[0].ClinicYmd,
                            WeekNum = schedule[0].WeekNum,
                            WeekNm = weekName,
                            StartHour = schedule[0].StartHour,
                            StartMinute = schedule[0].StartMinute,
                            EndHour = schedule[0].EndHour,
                            EndMinute = schedule[0].EndMinute,
                            BreakStartHour = schedule[0].BreakStartHour,
                            BreakStartMinute = schedule[0].BreakStartMinute,
                            BreakEndHour = schedule[0].BreakEndHour,
                            BreakEndMinute = schedule[0].BreakEndMinute,
                            IntervalTime = schedule[0].IntervalTime,
                            Hello100Role = schedule[0].Hello100Role,
                            Ridx = schedule[0].Ridx,
                            UseYn = schedule[0].UseYn,
                            RsrvCnt = schedule[0].RsrvCnt,
                            UntactStartHour = doctorScheduleResult[0].UntactStartHour,
                            UntactStartMinute = doctorScheduleResult[0].UntactStartMinute,
                            UntactEndHour = doctorScheduleResult[0].UntactEndHour,
                            UntactEndMinute = doctorScheduleResult[0].UntactEndMinute,
                            UntactIntervalTime = doctorScheduleResult[0].UntactIntervalTime,
                            UntactBreakStartHour = doctorScheduleResult[0].UntactBreakStartHour,
                            UntactBreakEndHour = doctorScheduleResult[0].UntactBreakEndHour,
                            UntactBreakStartMinute = doctorScheduleResult[0].UntactBreakStartMinute,
                            UntactBreakEndMinute = doctorScheduleResult[0].UntactBreakEndMinute,
                            UntactRsrvCnt = doctorScheduleResult[0].UntactRsrvCnt,
                            UntactUseYn = doctorScheduleResult[0].UntactUseYn
                        });

                        untactWeeksScheduleList.Add(new DoctorScheduleResult
                        {
                            HospNo = schedule[0].HospNo,
                            HospKey = schedule[0].HospKey,
                            EmplNo = schedule[0].EmplNo,
                            ClinicYmd = schedule[0].ClinicYmd,
                            WeekNum = schedule[0].WeekNum,
                            WeekNm = weekName,
                            StartHour = schedule[0].StartHour,
                            StartMinute = schedule[0].StartMinute,
                            EndHour = schedule[0].EndHour,
                            EndMinute = schedule[0].EndMinute,
                            BreakStartHour = schedule[0].BreakStartHour,
                            BreakStartMinute = schedule[0].BreakStartMinute,
                            BreakEndHour = schedule[0].BreakEndHour,
                            BreakEndMinute = schedule[0].BreakEndMinute,
                            IntervalTime = schedule[0].IntervalTime,
                            Hello100Role = schedule[0].Hello100Role,
                            Ridx = schedule[0].Ridx,
                            UseYn = schedule[0].UseYn,
                            RsrvCnt = schedule[0].RsrvCnt,
                            UntactStartHour = schedule[0].UntactStartHour,
                            UntactStartMinute = schedule[0].UntactStartMinute,
                            UntactEndHour = schedule[0].UntactEndHour,
                            UntactEndMinute = schedule[0].UntactEndMinute,
                            UntactIntervalTime = schedule[0].UntactIntervalTime,
                            UntactBreakStartHour = schedule[0].UntactBreakStartHour,
                            UntactBreakEndHour = schedule[0].UntactBreakEndHour,
                            UntactBreakStartMinute = schedule[0].UntactBreakStartMinute,
                            UntactBreakEndMinute = schedule[0].UntactBreakEndMinute,
                            UntactRsrvCnt = schedule[0].UntactRsrvCnt,
                            UntactUseYn = schedule[0].UntactUseYn
                        });
                    }
                }

                //지정스케줄
                doctorScheduleResult.Where(w => w.WeekNum == 11)
                    .OrderBy(s => s.ClinicYmd)
                    .ToList()
                    .ForEach(x =>
                    {
                        daysScheduleList.Add(new DoctorScheduleResult
                        {
                            HospNo = x.HospNo,
                            HospKey = x.HospKey,
                            EmplNo = x.EmplNo,
                            ClinicYmd = x.ClinicYmd,
                            WeekNum = x.WeekNum,
                            WeekNm = "예외",
                            StartHour = x.StartHour,
                            StartMinute = x.StartMinute,
                            EndHour = x.EndHour,
                            EndMinute = x.EndMinute,
                            BreakStartHour = x.BreakStartHour,
                            BreakStartMinute = x.BreakStartMinute,
                            BreakEndHour = x.BreakEndHour,
                            BreakEndMinute = x.BreakEndMinute,
                            IntervalTime = x.IntervalTime,
                            Hello100Role = x.Hello100Role,
                            Ridx = x.Ridx,
                            UseYn = x.UseYn,
                            RsrvCnt = x.RsrvCnt,
                            UntactStartHour = x.UntactStartHour,
                            UntactStartMinute = x.UntactStartMinute,
                            UntactEndHour = x.UntactEndHour,
                            UntactEndMinute = x.UntactEndMinute,
                            UntactIntervalTime = x.UntactIntervalTime,
                            UntactBreakStartHour = x.UntactBreakStartHour,
                            UntactBreakEndHour = x.UntactBreakEndHour,
                            UntactBreakStartMinute = x.UntactBreakStartMinute,
                            UntactBreakEndMinute = x.UntactBreakEndMinute,
                            UntactRsrvCnt = x.UntactRsrvCnt,
                            UntactUseYn = x.UntactUseYn
                        });
                    });
            }

            GetDoctorResult? result = new GetDoctorResult()
            {
                DoctorInfo = doctorInfo,
                WeeksScheduleList = weeksScheduleList,
                DaysScheduleList = daysScheduleList,
                UntactWeeksScheduleList = untactWeeksScheduleList
            };

            return Result.Success<GetDoctorResult?>(result);
        }
    }
}
