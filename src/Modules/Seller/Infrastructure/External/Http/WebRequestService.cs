using System.Net.Http.Headers;
using System.Text;
using System.Net.Mime;
using Hello100Admin.Modules.Seller.Infrastructure.External.Http.Models;

namespace Hello100Admin.Modules.Seller.Infrastructure.External.Http
{
    public sealed class WebRequestService : IWebRequestService
    {
        private readonly HttpClient _http;

        public WebRequestService(HttpClient http)
        {
            _http = http;

            _http.Timeout = Timeout.InfiniteTimeSpan;
        }

        public async Task<WebRequestResult> PostAsync(string uri, 
                                                      string sendData,
                                                      string? authHeader = null,
                                                      string contentType = MediaTypeNames.Application.Json,
                                                      int timeoutMs = 8000,
                                                      CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(sendData ?? string.Empty, Encoding.UTF8, contentType)
            };

            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                if (authHeader.Contains(' '))
                {
                    var parts = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 2)
                        request.Headers.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
                    else
                        request.Headers.TryAddWithoutValidation("Authorization", authHeader);
                }
                else
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authHeader);
                }
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));

            try
            {
                using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);

                var body = await response.Content.ReadAsStringAsync(cts.Token);

                return new WebRequestResult
                {
                    StatusCode = (int)response.StatusCode,
                    HeaderDate = response.Headers.Date?.ToString(),
                    ResponseData = body
                };
            }
            catch (OperationCanceledException e) when (cancellationToken.IsCancellationRequested == false)
            {
                return new WebRequestResult
                {
                    StatusCode = 0,
                    ResponseData = $"Timeout after {timeoutMs}ms: {e.Message}"
                };
            }
            catch (HttpRequestException e)
            {
                return new WebRequestResult
                {
                    StatusCode = 0,
                    ResponseData = e.Message
                };
            }
        }
    }
}
