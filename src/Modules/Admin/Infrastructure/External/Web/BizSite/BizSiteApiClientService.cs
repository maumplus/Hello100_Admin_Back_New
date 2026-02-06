using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using System.Text;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using System.Net.Mime;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.BizSite;

namespace Hello100Admin.Modules.Admin.Infrastructure.External.Web.BizSite
{
    public class BizSiteApiClientService : IBizSiteApiClientService
    {
        private readonly ILogger<BizSiteApiClientService> _logger;
        private readonly HttpClient _client;
        private readonly ICryptoService _cryptoService;

        public BizSiteApiClientService(ILogger<BizSiteApiClientService> logger, HttpClient http, ICryptoService cryptoService)
        {
            _logger = logger;
            _client = http;
            _cryptoService = cryptoService;
        }

        public async Task<BizSiteApiResult<KakaoMsgResultData>> PostKakaoMessageInfoAsync(string hospNo, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Process PostAsync() started.");

                string requestUrl = $"{_client.BaseAddress}/external/hello100/kakaomsginfo";

                var requestData = new
                {
                    hospCd = hospNo,
                    encKey = _cryptoService.EncryptWithNoVector("clinic2013!@")
                };

                string json = requestData.ToJson();
                var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

                HttpResponseMessage response = await _client.PostAsync(requestUrl, content);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var responseObj = jsonResponse.FromJson<BizSiteApiResult<KakaoMsgResultData>>();

                if (responseObj == null)
                    throw new BizException(AdminErrorCode.BizSiteDataRequestFailed.ToError());

                return responseObj;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Process PostAsync() Error");
                throw new BizException(AdminErrorCode.BizSiteDataRequestFailed.ToError());
            }
        }

        public async Task<KakaoMsgExaminationResultData> PostKakaoMessageExamiantionInfoAsync(string hospNo, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Process PostAsync() started.");

                string requestUrl = $"{_client.BaseAddress}/external/hello100/testresultyn";

                var requestData = new
                {
                    hospCd = hospNo,
                    encKey = _cryptoService.EncryptWithNoVector("clinic2013!@")
                };

                string json = requestData.ToJson();
                var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

                HttpResponseMessage response = await _client.PostAsync(requestUrl, content);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var responseObj = jsonResponse.FromJson<KakaoMsgExaminationResultData>();

                if (responseObj == null)
                    throw new BizException(AdminErrorCode.BizSiteDataRequestFailed.ToError());

                return responseObj;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Process PostAsync() Error");
                throw new BizException(AdminErrorCode.BizSiteDataRequestFailed.ToError());
            }
        }
    }
}
