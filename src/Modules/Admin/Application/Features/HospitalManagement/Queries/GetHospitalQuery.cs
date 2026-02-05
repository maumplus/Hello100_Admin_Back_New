using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    /// <summary>
    /// 병원정보 조회 쿼리
    /// </summary>
    /// <param name="HospNo">요양기관번호</param>
    public record GetHospitalQuery(string HospNo) : IRequest<Result<GetHospitalResult?>>;

    public class GetHospitalQueryHandler : IRequestHandler<GetHospitalQuery, Result<GetHospitalResult?>>
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

        public async Task<Result<GetHospitalResult?>> Handle(GetHospitalQuery query, CancellationToken cancellationToken)
        {
            var result = await _hospitalStore.GetHospital(query.HospNo, cancellationToken);

            if (result != null)
            {
                result.ClinicTimes = await _hospitalStore.GetHospMedicalTimeList(result.HospKey, cancellationToken);
                result.DeptCodes = await _hospitalStore.GetHospitalMedicalList(result.HospKey, cancellationToken);
                result.Keywords = await _hospitalStore.GetHospKeywordList(result.HospKey, cancellationToken);
                result.Images = await _hospitalStore.GetImageList(result.HospKey, cancellationToken);
                result.ClinicTimesNew = await _hospitalStore.GetHospMedicalTimeNewList(result.HospKey, cancellationToken);
                result.KeywordMasters = await _hospitalStore.GetKeywordMasterList(result.HospKey, cancellationToken);
            }

            return Result.Success(result);
        }
    }
}
