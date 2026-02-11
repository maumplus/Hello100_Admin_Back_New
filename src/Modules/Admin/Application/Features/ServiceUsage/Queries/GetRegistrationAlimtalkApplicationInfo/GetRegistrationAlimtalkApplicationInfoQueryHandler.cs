using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
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
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly IDbSessionRunner _db;

        public GetRegistrationAlimtalkApplicationInfoQueryHandler(IConfiguration config,
                                                                  ILogger<GetRegistrationAlimtalkApplicationInfoQueryHandler> logger,
                                                                  ICurrentHospitalProfileProvider currentHospitalProfileProvider,
                                                                  IServiceUsageStore serviceUsageStore,
                                                                  IDbSessionRunner db)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _kakaoSampleImagePath = config["KakaoRegistrationSampleImagePath"] ?? string.Empty;
            _logger = logger;
            _currentHospitalProfileProvider = currentHospitalProfileProvider;
            _serviceUsageStore = serviceUsageStore;
            _db = db;
        }

        public async Task<Result<GetRegistrationAlimtalkApplicationInfoResponse>> Handle(GetRegistrationAlimtalkApplicationInfoQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Process GetRegistrationAlimtalkApplicationInfoQueryHandler() started.");

            // 알림톡 발송 서비스 신청 정보
            var requestInfo = await _db.RunAsync(DataSource.Hello100,
                // 알림톡 진료접수는 tmpType이 ""(빈값)으로 고정
                (session, token) => _serviceUsageStore.FindAlimtalkServiceApplicationAsync(session, req.HospNo, req.HospKey, "", token),
            ct);

            var currentHospitalInfo = await _currentHospitalProfileProvider.GetCurrentHospitalProfileByHospNoAsync(req.HospNo, ct);

            var response = currentHospitalInfo.Adapt<GetRegistrationAlimtalkApplicationInfoResponse>() with
            {
                KakaoSampleImageUrl = $"{_adminImageUrl}{_kakaoSampleImagePath}",
                IsAlimtalkServiceApplied = requestInfo != null,
                DoctNm = requestInfo?.DoctNm ?? string.Empty,
                DoctTel = requestInfo?.DoctTel ?? string.Empty
            };

            return Result.Success(response);
        }
    }
}
