using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Request;
using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Response;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

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

        public async Task<KakaoBizResult<KakaoMsgSendHistoryDataSet>?> SendHistoryAsync(KakaoBizSendHistoryRequest req, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Process SendHistoryAsync() started.");

                string requestUrl = $"{_client.BaseAddress}ws/api/kakao/hello100/send/history?HospCd={req.HospNo}&DateFrom={req.FromDate}&DateTo={req.ToDate}";

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                requestMessage.Headers.Add("AuthPass", req.EncKey);

                HttpResponseMessage response = await _client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                // 응답 처리
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResultList = jsonResponse.FromJson<KakaoBizResult<KakaoMsgSendHistoryDataSet>>();

                return apiResultList;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Process SendAsync() Error");
                throw new BizException(AdminErrorCode.KakaoBizDataRequestFailed.ToError());
            }
        }

        public async Task LoginOtpAsync(KakaoBizLoginOtpRequest req, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Process LoginOtpAsync() started.");

                string requestUrl = $"{_client.BaseAddress}ws/api/kakao/hello100/admin/login/otp";

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(
                        req.ToJson(),
                        Encoding.Default
                    )
                };

                requestMessage.Headers.Add("AuthPass", req.EncKey);
                requestMessage.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                HttpResponseMessage response = await _client.SendAsync(requestMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Process LoginOtpAsync() Error");
                throw new BizException(AdminErrorCode.KakaoBizDataRequestFailed.ToError());
            }
        }
    }
}
