using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetExaminationResultAlimtalkApplicationInfo;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetExaminationResultAlimtalkApplicationInfo
{
    public class GetExaminationResultAlimtalkApplicationInfoQueryHandler : IRequestHandler<GetExaminationResultAlimtalkApplicationInfoQuery, Result<GetExaminationResultAlimtalkApplicationInfoResponse>>
    {
        private readonly string _adminImageUrl;
        private readonly string _kakaoSampleImagePath;
        private readonly ILogger<GetExaminationResultAlimtalkApplicationInfoQueryHandler> _logger;
        private readonly ICurrentHospitalProfileProvider _currentHospitalProfileProvider;

        public GetExaminationResultAlimtalkApplicationInfoQueryHandler(IConfiguration config,
                                                                      ILogger<GetExaminationResultAlimtalkApplicationInfoQueryHandler> logger,
                                                                      ICurrentHospitalProfileProvider currentHospitalProfileProvider)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _kakaoSampleImagePath = config["KakaoExaminationResultSampleImagePath"] ?? string.Empty;
            _logger = logger;
            _currentHospitalProfileProvider = currentHospitalProfileProvider;
        }

        public async Task<Result<GetExaminationResultAlimtalkApplicationInfoResponse>> Handle(GetExaminationResultAlimtalkApplicationInfoQuery req, CancellationToken token)
        {
            _logger.LogInformation("Process GetExaminationResultAlimtalkApplicationInfoQueryHandler() started.");
            var currentHospitalInfo = await _currentHospitalProfileProvider.GetCurrentHospitalProfileByHospNoAsync(req.HospNo, token);

            var response = currentHospitalInfo.Adapt<GetExaminationResultAlimtalkApplicationInfoResponse>() with
            {
                KakaoSampleImageUrl = $"{_adminImageUrl}{_kakaoSampleImagePath}"
            };

            return Result.Success(response);
        }
    }
}
