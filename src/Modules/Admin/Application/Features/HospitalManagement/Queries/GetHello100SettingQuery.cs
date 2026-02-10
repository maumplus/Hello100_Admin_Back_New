using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
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
    /// <param name="HospNo"></param>
    /// <param name="HospKey"></param>
    public record GetHello100SettingQuery(string HospNo, string HospKey) : IRequest<Result<GetHello100SettingResult?>>;

    public class GetHello100SettingQueryHandler : IRequestHandler<GetHello100SettingQuery, Result<GetHello100SettingResult?>>
    {
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ICurrentHospitalProfileProvider _currentHospitalProfileProvider;
        private readonly IBizSiteApiClientService _bizSiteApiClientService;
        private readonly ILogger<GetHello100SettingQueryHandler> _logger;
        private readonly IDbSessionRunner _db;

        public GetHello100SettingQueryHandler(
            IHospitalManagementStore hospitalStore,
            ICurrentHospitalProfileProvider currentHospitalProfileProvider,
            IBizSiteApiClientService bizSiteApiClientService,
            ILogger<GetHello100SettingQueryHandler> logger,
            IDbSessionRunner db)
        {
            _hospitalStore = hospitalStore;
            _currentHospitalProfileProvider = currentHospitalProfileProvider;
            _bizSiteApiClientService = bizSiteApiClientService;
            _logger = logger;
            _db = db;
        }

        public async Task<Result<GetHello100SettingResult?>> Handle(GetHello100SettingQuery req, CancellationToken ct)
        {
            var hospInfo = await _db.RunAsync(DataSource.Hello100, 
                (dbSession, token) => _currentHospitalProfileProvider.GetCurrentHospitalProfileByHospNoAsync(req.HospNo, token)
            , ct);

            var result = await _db.RunAsync(DataSource.Hello100, 
                (dbSession, token) => _hospitalStore.GetHello100SettingAsync(dbSession, req.HospKey, token)
            , ct);

            if (result != null)
            {
                result.Name = hospInfo.Name;
                result.HospNo = hospInfo.HospNo;
                result.ChartType = hospInfo.ChartType;
                result.ReceptEndTime = !string.IsNullOrEmpty(result.ReceptEndTime) 
                    ? Convert.ToInt32(result.ReceptEndTime).ToString("##:##") 
                    : string.Empty;

                // 닉스 차트의 경우 사용 안함을 디폴트로 함
                if (hospInfo.ChartType == "N")
                    result.ExamPushSet = 9;

                var kakaoMsgInfo = await _bizSiteApiClientService.PostKakaoMessageInfoAsync(req.HospNo, ct);

                result.SendYn = kakaoMsgInfo.ResultData?.SendYn != "Y" ? "N" : "Y";
                result.SendStartYmd = kakaoMsgInfo.ResultData?.SendStartYmd;
                result.SendEndYmd = kakaoMsgInfo.ResultData?.SendEndYmd;

                var kakaoMsgExamInfo = await _bizSiteApiClientService.PostKakaoMessageExamiantionInfoAsync(req.HospNo, ct);

                result.ExamApproveYn = string.IsNullOrWhiteSpace(kakaoMsgExamInfo.ResultData) ? "N" : kakaoMsgExamInfo.ResultData;
            }

            return Result.Success(result);
        }
    }
}
