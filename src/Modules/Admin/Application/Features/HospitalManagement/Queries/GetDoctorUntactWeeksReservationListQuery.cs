using DocumentFormat.OpenXml.ExtendedProperties;
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
    public class GetDoctorUntactWeeksReservationListQuery : IRequest<Result<GetDoctorUntactWeeksReservationListResult>>
    {
        [JsonIgnore]
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public string ReCalculateYn { get; set; }
        public string UntactStartTime { get; set; }
        public string UntactEndTime { get; set; }
        public string UntactBreakStartTime { get; set; }
        public string UntactBreakEndTime { get; set; }
        public int UntactRsrvIntervalTime { get; set; }
        public int UntactRsrvIntervalCnt { get; set; }
    }

    public class GetDoctorUntactWeeksReservationListQueryHandler : IRequestHandler<GetDoctorUntactWeeksReservationListQuery, Result<GetDoctorUntactWeeksReservationListResult>>
    {
        private readonly ICryptoService _cryptoService;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetDoctorUntactWeeksReservationListQueryHandler> _logger;

        public GetDoctorUntactWeeksReservationListQueryHandler(
            ICryptoService cryptoService,
            IHospitalManagementStore hospitalStore,
            ILogger<GetDoctorUntactWeeksReservationListQueryHandler> logger)
        {
            _cryptoService = cryptoService;
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<GetDoctorUntactWeeksReservationListResult>> Handle(GetDoctorUntactWeeksReservationListQuery query, CancellationToken cancellationToken)
        {
            var eghisDoctRsrvInfoEntity = await _hospitalStore.GetEghisDoctRsrvInfo(query.HospNo, query.EmplNo, query.WeekNum, string.Empty, cancellationToken);

            var eghisDoctRsrvDetailEntityList = new List<EghisDoctRsrvDetailInfoEntity>();

            if (eghisDoctRsrvInfoEntity == null)
            {
                eghisDoctRsrvInfoEntity = new EghisDoctRsrvInfoEntity()
                {
                    HospNo = query.HospNo,
                    EmplNo = query.EmplNo,
                    WeekNum = query.WeekNum,
                    ClinicYmd = string.Empty,
                    RsrvIntervalTime = 10,
                    RsrvIntervalCnt = 0,
                    UntactRsrvIntervalTime = query.UntactRsrvIntervalTime == 0 ? 10 : query.UntactRsrvIntervalTime,
                    UntactRsrvIntervalCnt = query.UntactRsrvIntervalCnt == 0 ? 1 : query.UntactRsrvIntervalCnt,
                    UntactAvaUseYn = "N"
                };
            }
            else if (query.ReCalculateYn == "Y")
            {
                eghisDoctRsrvInfoEntity.UntactRsrvIntervalTime = query.UntactRsrvIntervalTime == 0 ? 10 : query.UntactRsrvIntervalTime;
                eghisDoctRsrvInfoEntity.UntactRsrvIntervalCnt = query.UntactRsrvIntervalCnt == 0 ? 1 : query.UntactRsrvIntervalCnt;
            }
            else
            {
                eghisDoctRsrvDetailEntityList = await _hospitalStore.GetEghisDoctRsrvDetailList(eghisDoctRsrvInfoEntity.Ridx, "NR", cancellationToken);
            }

            if (eghisDoctRsrvDetailEntityList.Count == 0 && (eghisDoctRsrvInfoEntity.UntactRsrvIntervalTime - 1) > 0)
            {
                TimeSpan time = new TimeSpan(00, eghisDoctRsrvInfoEntity.UntactRsrvIntervalTime, 00);
                TimeSpan addTime = new TimeSpan(00, eghisDoctRsrvInfoEntity.UntactRsrvIntervalTime - 1, 00);

                var untactStartDateTime = query.UntactStartTime.ToDateTime("HHmm");
                var untactEndDateTime = query.UntactEndTime.ToDateTime("HHmm");
                var untactBreakStartDateTime = query.UntactBreakStartTime.ToDateTime("HHmm");
                var untactBreakEndDateTime = query.UntactBreakEndTime.ToDateTime("HHmm");

                if (untactStartDateTime == null || untactEndDateTime == null || untactBreakStartDateTime == null || untactBreakEndDateTime == null)
                {

                }
                else if (untactStartDateTime.Value >= untactEndDateTime.Value)
                {
                    
                }
                else
                {
                    for (var i = untactStartDateTime.Value; i < untactEndDateTime.Value; i += time)
                    {
                        if (i >= untactBreakStartDateTime.Value && i < untactBreakEndDateTime.Value)
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
                            ReceptType = "NR"
                        };

                        eghisDoctRsrvDetailEntityList.Add(eghisDoctRsrvDetailInfoEntity);
                    }
                }
            }

            var eghisRsrvInfoEntityList = await _hospitalStore.GetEghisUntactRsrvList(query.HospNo, query.EmplNo, query.WeekNum, cancellationToken);

            foreach (var eghisRsrvInfoEntity in eghisRsrvInfoEntityList)
            {
                eghisRsrvInfoEntity.PtntNo = _cryptoService.DecryptWithNoVector(eghisRsrvInfoEntity.PtntNo);
                eghisRsrvInfoEntity.PtntNm = _cryptoService.DecryptWithNoVector(eghisRsrvInfoEntity.PtntNm);
            }

            var result = new GetDoctorUntactWeeksReservationListResult()
            {
                EghisDoctRsrvInfo = eghisDoctRsrvInfoEntity,
                EghisDoctRsrvDetailList = eghisDoctRsrvDetailEntityList,
                EghisUntactRsrvList = eghisRsrvInfoEntityList
            };

            return Result.Success(result);
        }
    }
}
