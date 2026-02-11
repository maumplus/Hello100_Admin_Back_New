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
    public class GetDoctorWeeksReservationListQuery : IRequest<Result<GetDoctorWeeksReservationListResult>>
    {
        [JsonIgnore]
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public int WeekNum { get; set; }
    }

    public class GetDoctorWeeksReservationListQueryHandler : IRequestHandler<GetDoctorWeeksReservationListQuery, Result<GetDoctorWeeksReservationListResult>>
    {
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetDoctorWeeksReservationListQueryHandler> _logger;

        public GetDoctorWeeksReservationListQueryHandler(
        IHospitalManagementStore hospitalStore,
        ILogger<GetDoctorWeeksReservationListQueryHandler> logger)
        {
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<GetDoctorWeeksReservationListResult>> Handle(GetDoctorWeeksReservationListQuery query, CancellationToken cancellationToken)
        {
            var eghisDoctRsrvInfoEntity = await _hospitalStore.GetEghisDoctRsrvInfo(query.HospNo, query.EmplNo, query.WeekNum, string.Empty, cancellationToken);

            var eghisDoctRsrvDetailEntityList = new List<EghisDoctRsrvDetailInfoEntity>();
            var eghisRsrvInfoEntityList = new List<EghisRsrvInfoEntity>();

            if (eghisDoctRsrvInfoEntity != null)
            {
                eghisDoctRsrvDetailEntityList = await _hospitalStore.GetEghisDoctRsrvDetailList(eghisDoctRsrvInfoEntity.Ridx, "RS", cancellationToken);
                eghisRsrvInfoEntityList = await _hospitalStore.GetEghisRsrvList(query.HospNo, query.EmplNo, query.WeekNum, cancellationToken);
            }
            
            var result = new GetDoctorWeeksReservationListResult()
            {
                EghisDoctRsrvInfo = eghisDoctRsrvInfoEntity,
                EghisDoctRsrvDetailList = eghisDoctRsrvDetailEntityList,
                EghisRsrvList = eghisRsrvInfoEntityList
            };

            return Result.Success(result);
        }
    }
}
