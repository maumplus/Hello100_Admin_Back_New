using Hello100Admin.BuildingBlocks.Common.Application;
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
    }

    public class GetDoctorUntactWeeksReservationListQueryHandler : IRequestHandler<GetDoctorUntactWeeksReservationListQuery, Result<GetDoctorUntactWeeksReservationListResult>>
    {
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetDoctorUntactWeeksReservationListQueryHandler> _logger;

        public GetDoctorUntactWeeksReservationListQueryHandler(
        IHospitalManagementStore hospitalStore,
        ILogger<GetDoctorUntactWeeksReservationListQueryHandler> logger)
        {
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<GetDoctorUntactWeeksReservationListResult>> Handle(GetDoctorUntactWeeksReservationListQuery query, CancellationToken cancellationToken)
        {
            var eghisDoctRsrvInfoEntity = await _hospitalStore.GetEghisDoctRsrvInfo(query.HospNo, query.EmplNo, query.WeekNum, string.Empty, cancellationToken);

            var eghisDoctRsrvDetailEntityList = new List<EghisDoctRsrvDetailInfoEntity>();
            var eghisRsrvInfoEntityList = new List<EghisRsrvInfoEntity>();

            if (eghisDoctRsrvInfoEntity != null)
            {
                eghisDoctRsrvDetailEntityList = await _hospitalStore.GetEghisDoctRsrvDetailList(eghisDoctRsrvInfoEntity.Ridx, "NR", cancellationToken);
                eghisRsrvInfoEntityList = await _hospitalStore.GetEghisUntactRsrvList(query.HospNo, query.EmplNo, query.WeekNum, cancellationToken);
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
