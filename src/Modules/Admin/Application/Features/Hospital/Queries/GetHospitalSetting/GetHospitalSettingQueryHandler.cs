using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.ReadModels;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Responses;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetHospitalSetting
{
    public class GetHospitalSettingQueryHandler : IRequestHandler<GetHospitalSettingQuery, Result<GetHospitalSettingResponse?>>
    {
        private readonly IHospitalStore _hospitalStore;
        private readonly ILogger<GetHospitalSettingQueryHandler> _logger;

        public GetHospitalSettingQueryHandler(
        IHospitalStore hospitalStore,
        ILogger<GetHospitalSettingQueryHandler> logger)
        {
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<GetHospitalSettingResponse?>> Handle(GetHospitalSettingQuery query, CancellationToken cancellationToken)
        {
            GetHospitalSettingResponse? result = null;

            var hospital = await _hospitalStore.GetHospital(query.HospNo, cancellationToken);

            if (hospital != null)
            {
                var hospitalSetting = await _hospitalStore.GetHospSetting(hospital.HospKey, cancellationToken);

                if (hospitalSetting != null)
                {
                    result = hospitalSetting.Adapt<GetHospitalSettingResponse>();
                }
            }

            return Result.Success<GetHospitalSettingResponse?>(result);
        }
    }
}
