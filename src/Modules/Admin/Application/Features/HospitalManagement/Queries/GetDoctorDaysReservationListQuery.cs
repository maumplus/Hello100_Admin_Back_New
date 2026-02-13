using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Hello100Admin.Modules.Admin.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public class GetDoctorDaysReservationListQuery : IRequest<Result<GetDoctorDaysReservationListResult>>
    {
        [JsonIgnore]
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public string ClinicYmd { get; set; }
        public string ReCalculateYn { get; set; } = "N";
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string BreakStartTime { get; set; }
        public string BreakEndTime { get; set; }
        public int RsrvIntervalTime { get; set; }
        public int RsrvIntervalCnt { get; set; }
    }

    public class GetDoctorDaysReservationListQueryHandler : IRequestHandler<GetDoctorDaysReservationListQuery, Result<GetDoctorDaysReservationListResult>>
    {
        private readonly ICryptoService _cryptoService;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetDoctorDaysReservationListQueryHandler> _logger;

        public GetDoctorDaysReservationListQueryHandler(
            ICryptoService cryptoService,
            IHospitalManagementStore hospitalStore,
            ILogger<GetDoctorDaysReservationListQueryHandler> logger)
        {
            _cryptoService = cryptoService;
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<GetDoctorDaysReservationListResult>> Handle(GetDoctorDaysReservationListQuery query, CancellationToken cancellationToken)
        {
            var eghisDoctRsrvInfoEntity = await _hospitalStore.GetEghisDoctRsrvInfo(query.HospNo, query.EmplNo, 11, query.ClinicYmd, cancellationToken);

            var eghisDoctRsrvDetailEntityList = new List<EghisDoctRsrvDetailInfoEntity>();

            if (eghisDoctRsrvInfoEntity == null)
            {
                eghisDoctRsrvInfoEntity = new EghisDoctRsrvInfoEntity()
                {
                    HospNo = query.HospNo,
                    EmplNo = query.EmplNo,
                    WeekNum = 11,
                    ClinicYmd = query.ClinicYmd,
                    RsrvIntervalTime = query.RsrvIntervalTime == 0 ? 10 : query.RsrvIntervalTime,
                    RsrvIntervalCnt = query.RsrvIntervalCnt
                };
            }
            else if (query.ReCalculateYn == "Y")
            {
                eghisDoctRsrvInfoEntity.RsrvIntervalTime = query.RsrvIntervalTime == 0 ? 10 : query.RsrvIntervalTime;
                eghisDoctRsrvInfoEntity.RsrvIntervalCnt = query.RsrvIntervalCnt;
            }
            else
            {
                eghisDoctRsrvDetailEntityList = await _hospitalStore.GetEghisDoctRsrvDetailList(eghisDoctRsrvInfoEntity.Ridx, "RS", cancellationToken);
            }

            if (eghisDoctRsrvDetailEntityList.Count == 0 && (eghisDoctRsrvInfoEntity.RsrvIntervalTime - 1) > 0)
            {
                TimeSpan time = new TimeSpan(00, eghisDoctRsrvInfoEntity.RsrvIntervalTime, 00);
                TimeSpan addTime = new TimeSpan(00, eghisDoctRsrvInfoEntity.RsrvIntervalTime - 1, 00);

                var startDateTime = query.StartTime.ToDateTime("HHmm");
                var endDateTime = query.EndTime.ToDateTime("HHmm");
                var breakStartDateTime = query.BreakStartTime.ToDateTime("HHmm");
                var breakEndDateTime = query.BreakEndTime.ToDateTime("HHmm");

                if (startDateTime == null || endDateTime == null || breakStartDateTime == null || breakEndDateTime == null)
                {

                }
                else if (startDateTime.Value >= endDateTime.Value)
                {

                }
                else
                {
                    for (var i = startDateTime.Value; i < endDateTime.Value; i += time)
                    {
                        if (i >= breakStartDateTime.Value && i < breakEndDateTime.Value)
                        {
                            continue;
                        }

                        var eghisDoctRsrvDetailInfoEntity = new EghisDoctRsrvDetailInfoEntity()
                        {
                            Ridx = eghisDoctRsrvInfoEntity.Ridx,
                            StartTime = i.ToString("HHmm"),
                            EndTime = (i + addTime).ToString("HHmm"),
                            RsrvCnt = eghisDoctRsrvInfoEntity.RsrvIntervalCnt,
                            ComCnt = 0,
                            ReceptType = "RS"
                        };

                        eghisDoctRsrvDetailEntityList.Add(eghisDoctRsrvDetailInfoEntity);
                    }
                }
            }

            var eghisRsrvInfoEntityList = await _hospitalStore.GetEghisRsrvList(query.HospNo, query.EmplNo, query.ClinicYmd, cancellationToken);

            foreach (var eghisRsrvInfoEntity in eghisRsrvInfoEntityList)
            {
                eghisRsrvInfoEntity.PtntNo = _cryptoService.DecryptWithNoVector(eghisRsrvInfoEntity.PtntNo);
                eghisRsrvInfoEntity.PtntNm = _cryptoService.DecryptWithNoVector(eghisRsrvInfoEntity.PtntNm);
            }

            var result = new GetDoctorDaysReservationListResult()
            {
                EghisDoctRsrvInfo = eghisDoctRsrvInfoEntity,
                EghisDoctRsrvDetailList = eghisDoctRsrvDetailEntityList,
                EghisRsrvList = eghisRsrvInfoEntityList
            };

            return Result.Success(result);
        }
    }
}
