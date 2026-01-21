using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Request;
using Hello100Admin.Modules.Admin.Application.Common.Constracts.External.Web.KakaoBiz.Response;

namespace Hello100Admin.Modules.Admin.Application.Common.Abstractions.External
{
    public interface IBizApiClientService
    {
        public Task<KakaoBizResult<KakaoMsgSendHistoryDataSet>?> SendAsync(KakaoBizRequest req, CancellationToken ct);
    }
}
