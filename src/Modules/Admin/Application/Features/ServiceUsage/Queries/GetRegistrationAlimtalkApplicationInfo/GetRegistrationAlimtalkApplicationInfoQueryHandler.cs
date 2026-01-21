using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetRegistrationAlimtalkApplicationInfo;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetRegistrationAlimtalkApplicationInfo
{
    public class GetRegistrationAlimtalkApplicationInfoQueryHandler : IRequestHandler<GetRegistrationAlimtalkApplicationInfoQuery, Result<GetRegistrationAlimtalkApplicationInfoResponse>>
    {
        private readonly string _adminImageUrl;
        private readonly string _kakaoSampleImagePath;
        private readonly ILogger<GetRegistrationAlimtalkApplicationInfoQueryHandler> _logger;
        private readonly ICurrentHospitalProfileProvider _currentHospitalProfileProvider;

        public GetRegistrationAlimtalkApplicationInfoQueryHandler(IConfiguration config,
                                                                  ILogger<GetRegistrationAlimtalkApplicationInfoQueryHandler> logger,
                                                                  ICurrentHospitalProfileProvider currentHospitalProfileProvider)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _kakaoSampleImagePath = config["KakaoRegistrationSampleImagePath"] ?? string.Empty;
            _logger = logger;
            _currentHospitalProfileProvider = currentHospitalProfileProvider;
        }

        public async Task<Result<GetRegistrationAlimtalkApplicationInfoResponse>> Handle(GetRegistrationAlimtalkApplicationInfoQuery req, CancellationToken token)
        {
            _logger.LogInformation("Process GetRegistrationAlimtalkApplicationInfoQueryHandler() started.");
            var currentHospitalInfo = await _currentHospitalProfileProvider.GetCurrentHospitalProfileByHospNoAsync(req.HospNo, token);

            var response = currentHospitalInfo.Adapt<GetRegistrationAlimtalkApplicationInfoResponse>() with
            {
                KakaoSampleImageUrl = $"{_adminImageUrl}{_kakaoSampleImagePath}"
            };

            return Result.Success(response);
        }
    }
}
