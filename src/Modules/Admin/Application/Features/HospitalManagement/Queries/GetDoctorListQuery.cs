using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    /// <summary>
    /// 의료진 목록 조회 쿼리
    /// </summary>
    public class GetDoctorListQuery : IRequest<Result<List<GetDoctorListResult>>>
    {
        public string HospNo { get; set; } = string.Empty;
    }

    public class GetDoctorListQueryHandler : IRequestHandler<GetDoctorListQuery, Result<List<GetDoctorListResult>>>
    {
        private readonly ICryptoService _cryptoService;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetDoctorListQueryHandler> _logger;

        public GetDoctorListQueryHandler(
            ICryptoService cryptoService,
            IHospitalManagementStore hospitalStore,
            ILogger<GetDoctorListQueryHandler> logger)
        {
            _cryptoService = cryptoService;
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<List<GetDoctorListResult>>> Handle(GetDoctorListQuery query, CancellationToken cancellationToken)
        {
            var doctorList = await _hospitalStore.GetDoctorList(query.HospNo, cancellationToken);

            foreach (var doctor in doctorList)
            {
                doctor.DoctNo = _cryptoService.DecryptWithNoVector(doctor.DoctNo);
            }

            var result = doctorList.Adapt<List<GetDoctorListResult>>();

            return Result.Success(result);
        }
    }
}