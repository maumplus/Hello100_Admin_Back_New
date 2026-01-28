using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetDoctorList;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Responses;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.Hospital;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetHospital
{
    public class GetHospitalQueryHandler : IRequestHandler<GetHospitalQuery, Result<GetHospitalResponse>>
    {
        private readonly IHospitalStore _hospitalStore;
        private readonly ILogger<GetHospitalQueryHandler> _logger;

        public GetHospitalQueryHandler(
        IHospitalStore hospitalStore,
        ILogger<GetHospitalQueryHandler> logger)
        {
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<GetHospitalResponse>> Handle(GetHospitalQuery query, CancellationToken cancellationToken)
        {
            var hospital = await _hospitalStore.GetHospital(query.HospNo, cancellationToken);

            if (hospital != null)
            {
                hospital.ClinicTimes = await _hospitalStore.GetHospMedicalTimeList(hospital.HospKey, cancellationToken);
                hospital.DeptCodes = await _hospitalStore.GetHospitalMedicalList(hospital.HospKey, cancellationToken);
                hospital.Keywords = await _hospitalStore.GetHospKeywordList(hospital.HospKey, cancellationToken);
                hospital.Images = await _hospitalStore.GetImageList(hospital.HospKey, cancellationToken);
                hospital.ClinicTimeNews = await _hospitalStore.GetHospMedicalTimeNewList(hospital.HospKey, cancellationToken);
                hospital.KeywordMasters = await _hospitalStore.GetKeywordMasterList(hospital.HospKey, cancellationToken);
            }

            var result = hospital.Adapt<GetHospitalResponse>();

            return Result.Success<GetHospitalResponse>(result);
        }
    }
}
