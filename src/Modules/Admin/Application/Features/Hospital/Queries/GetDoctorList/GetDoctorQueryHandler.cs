using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Responses;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetDoctorList
{
    public class GetDoctorListQueryHandler : IRequestHandler<GetDoctorListQuery, Result<List<GetDoctorListResponse>>>
    {
        private readonly IHospitalStore _hospitalStore;
        private readonly ILogger<GetDoctorListQueryHandler> _logger;

        public GetDoctorListQueryHandler(
        IHospitalStore hospitalStore,
        ILogger<GetDoctorListQueryHandler> logger)
        {
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<List<GetDoctorListResponse>>> Handle(GetDoctorListQuery query, CancellationToken cancellationToken)
        {
            var doctorList = await _hospitalStore.GetDoctorList(query.HospNo, cancellationToken);

            var result = doctorList.Adapt<List<GetDoctorListResponse>>();

            return Result.Success<List<GetDoctorListResponse>>(result);
        }
    }
}
