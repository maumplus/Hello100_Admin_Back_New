using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Request;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.SearchExaminationResultAlimtalkHistories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchExaminationResultAlimtalkHistories
{
    public class SearchExaminationResultAlimtalkHistoriesQueryHandler : IRequestHandler<SearchExaminationResultAlimtalkHistoriesQuery, Result<SearchExaminationResultAlimtalkHistoriesResponse>>
    {
        private readonly ILogger<SearchExaminationResultAlimtalkHistoriesQueryHandler> _logger;
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly ICryptoService _cryptoService;
        private readonly IBizApiClientService _bizApiClientService;

        public SearchExaminationResultAlimtalkHistoriesQueryHandler(ILogger<SearchExaminationResultAlimtalkHistoriesQueryHandler> logger, 
                                                                           IServiceUsageStore serviceUsageStore,
                                                                           ICryptoService cryptoService,
                                                                           IBizApiClientService bizApiClientService)
        {
            _logger = logger;
            _serviceUsageStore = serviceUsageStore;
            _cryptoService = cryptoService;
            _bizApiClientService = bizApiClientService;
        }

        public async Task<Result<SearchExaminationResultAlimtalkHistoriesResponse>> Handle(SearchExaminationResultAlimtalkHistoriesQuery req, CancellationToken token)
        {
            _logger.LogInformation("Process SearchDiagnosticTestResultAlimtalkSendHistoriesQuery() started.");
            var result = await _serviceUsageStore.SearchExaminationResultAlimtalkHistoriesAsync(req, token);

            var resultList = result.List;

            var kakaoBizRequest = new KakaoBizSendHistoryRequest
            {
                HospNo = req.HospNo,
                FromDate = Convert.ToDateTime(req.FromDate).ToString("yyyyMMdd"),
                ToDate = Convert.ToDateTime(req.ToDate).ToString("yyyyMMdd"),
                EncKey = _cryptoService.Encrypt("clinic2013!" + DateTime.Now.ToString("yyyyMMdd"))
            };

            var bizResult = await _bizApiClientService.SendHistoryAsync(kakaoBizRequest, token);

            if (bizResult != null && bizResult.ResultCd == 0 && bizResult.ResultData.ListCount > 0)
            {
                var joinedItems = resultList.Join(
                    bizResult.ResultData.List,
                    a => a.NotificationId?.ToUpper(),
                    b => b.HcResult?.ToUpper(),
                    (a, b) => new { ItemA = a, ItemB = b }
                    );

                foreach (var pair in joinedItems)
                {
                    if (pair.ItemB.ResultCd == "K000")
                    {
                        pair.ItemA.SendStatus = "발송성공";
                    }
                    else
                    {
                        pair.ItemA.SendStatus = "발송실패";
                        pair.ItemA.Message = $"{pair.ItemB.ResultCd} {pair.ItemB.ResultMsg}";
                    }
                }
            }

            if (req.SendStatus == 1)
            {
                resultList.RemoveAll(a => a.SendStatus != "발송성공");
            }
            else if (req.SendStatus == 2)
            {
                resultList.RemoveAll(a => a.SendStatus != "발송실패");
            }

            int totalSendCount = resultList.Count;
            int kakaoCount = resultList.Count(a => a.SendType == "카카오톡");

            result.TotalPtntCount = resultList.Select(a => a.PtntName).Distinct().Count();
            result.TotalSendCount = totalSendCount;
            result.SendSuccessCount = resultList.Count(a => a.SendStatus == "발송성공");
            result.SendFailCount = kakaoCount - result.SendSuccessCount;
            result.PushCount = resultList.Count(a => a.SendType == "App Push");
            result.KakaoCount = kakaoCount;

            var response = result.Adapt<SearchExaminationResultAlimtalkHistoriesResponse>();

            return Result.Success(response);
        }
    }
}
