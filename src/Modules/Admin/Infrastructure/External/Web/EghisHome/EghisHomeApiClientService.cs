using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;

namespace Hello100Admin.Modules.Admin.Infrastructure.External.Web.EghisHome
{
    public class EghisHomeApiClientService : IEghisHomeApiClientService
    {
        private readonly ILogger<EghisHomeApiClientService> _logger;
        private readonly HttpClient _client;

        public EghisHomeApiClientService(ILogger<EghisHomeApiClientService> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task RequestKakaoAlimTalkServiceAsync(string hospNo, string chartType, string tmpType, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Process SendAsync() started.");

                string requestUrl = $"{_client.BaseAddress}hello100/kakao/join/{hospNo}/{chartType}?TmpType={tmpType}";

                HttpResponseMessage response = await _client.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Process SendAsync() Error");
                throw new BizException(AdminErrorCode.RequestKakaoAlimTalkServiceFailed.ToError());
            }
        }
    }
}
