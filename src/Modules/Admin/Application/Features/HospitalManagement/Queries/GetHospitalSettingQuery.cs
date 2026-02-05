using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    /// <summary>
    /// Hello100 설정 조회 쿼리
    /// </summary>
    /// <param name="HospKey"></param>
    public record GetHospitalSettingQuery(string HospKey) : IRequest<Result<GetHospitalSettingResult?>>;

    public class GetHospitalSettingQueryHandler : IRequestHandler<GetHospitalSettingQuery, Result<GetHospitalSettingResult?>>
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

        public async Task<Result<GetHospitalSettingResult?>> Handle(GetHospitalSettingQuery req, CancellationToken cancellationToken)
        {
            GetHospitalSettingResult? result = null;

            var hospitalSetting = await _hospitalStore.GetHospSetting(req.HospKey, cancellationToken);

            if (hospitalSetting != null)
            {
                result = hospitalSetting.Adapt<GetHospitalSettingResult>();
            }

            return Result.Success(result);
        }
    }
}
