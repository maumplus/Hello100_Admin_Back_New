using System.Net.Mime;
using Hello100Admin.Modules.Seller.Infrastructure.External.Http.Models;

namespace Hello100Admin.Modules.Seller.Infrastructure.External.Http
{
    public interface IWebRequestService
    {
        public Task<WebRequestResult> PostAsync(string uri,
                                                    string sendData,
                                                    string? authHeader = null,
                                                    string contentType = MediaTypeNames.Application.Json,
                                                    int timeoutMs = 8000,
                                                    CancellationToken cancellationToken = default);
    }
}
