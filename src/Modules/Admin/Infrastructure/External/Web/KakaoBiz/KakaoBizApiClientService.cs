using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Request;
using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Response;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Hello100Admin.Modules.Admin.Infrastructure.External.Web.KakaoBiz
{
    public class KakaoBizApiClientService : IBizApiClientService
    {
        private readonly ILogger<KakaoBizApiClientService> _logger;
        private readonly HttpClient _client;

        public KakaoBizApiClientService(ILogger<KakaoBizApiClientService> logger, HttpClient http)
        {
            _logger = logger;
            _client = http;
        }

        public async Task<KakaoBizResult<KakaoMsgSendHistoryDataSet>?> SendAsync(KakaoBizRequest req, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Process SendAsync() started.");

                string requestUrl = $"{_client.BaseAddress}?HospCd={req.HospNo}&DateFrom={req.FromDate}&DateTo={req.ToDate}";

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                requestMessage.Headers.Add("AuthPass", req.EncKey);

                HttpResponseMessage response = await _client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                // 응답 처리
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResultList = JsonSerializer.Deserialize<KakaoBizResult<KakaoMsgSendHistoryDataSet>>(jsonResponse);

                return apiResultList;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Process SendAsync() Error");
                throw new BizException(AdminErrorCode.KakaoBizDataRequestFailed.ToError());
            }
        }
    }
}
