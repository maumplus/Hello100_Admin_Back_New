using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
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
        private readonly IHospitalInfoProvider _hospitalInfoProvider;
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly IDbSessionRunner _db;

        public GetExaminationResultAlimtalkApplicationInfoQueryHandler(IConfiguration config,
                                                                      ILogger<GetExaminationResultAlimtalkApplicationInfoQueryHandler> logger,
                                                                      IHospitalInfoProvider hospitalInfoProvider,
                                                                      IServiceUsageStore serviceUsageStore,
                                                                      IDbSessionRunner db)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _kakaoSampleImagePath = config["KakaoExaminationResultSampleImagePath"] ?? string.Empty;
            _logger = logger;
            _hospitalInfoProvider = hospitalInfoProvider;
            _serviceUsageStore = serviceUsageStore;
            _db = db;
        }

        public async Task<Result<GetExaminationResultAlimtalkApplicationInfoResponse>> Handle(GetExaminationResultAlimtalkApplicationInfoQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Process GetExaminationResultAlimtalkApplicationInfoQueryHandler() started.");

            // 알림톡 발송 서비스 신청 정보
            var requestInfo = await _db.RunAsync(DataSource.Hello100,
                // 알림톡 진료접수는 tmpType이 "KakaoJoinTestResult"으로 고정
                (session, token) => _serviceUsageStore.FindAlimtalkServiceApplicationAsync(session, req.HospNo, req.HospKey, "KakaoJoinTestResult", token),
            ct);

            var currentHospitalInfo = await _hospitalInfoProvider.GetHospitalInfoByHospNoAsync(req.HospNo, ct);

            if (currentHospitalInfo == null)
                return Result.Success<GetExaminationResultAlimtalkApplicationInfoResponse>().WithError(AdminErrorCode.NotFoundCurrentHospital.ToError());

            var response = currentHospitalInfo.Adapt<GetExaminationResultAlimtalkApplicationInfoResponse>() with
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
