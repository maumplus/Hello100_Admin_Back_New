using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        //protected IMediator Mediator =>
        //    HttpContext.RequestServices.GetRequiredService<IMediator>();

        protected string Aid => User.FindFirst("sub")?.Value ?? "";
        protected string AccId => User.FindFirst("accId")?.Value ?? "";
        protected string Name => User.FindFirst("name")?.Value ?? "";
        protected string Grade => User.FindFirst("grade")?.Value ?? "";
        protected string HospNo => User.FindFirst("hospNo")?.Value ?? "";
        protected string HospKey => User.FindFirst("hospKey")?.Value ?? "";
        protected string? UserAgent => this.GetClientUserAgent();
        protected string? ClientIpAddress => this.GetClientIpAddress();

        protected string? GetClientUserAgent()
        {
            string? userAgent = null;

            if (!string.IsNullOrEmpty(Request.Headers["User-Agent"]))
            {
                userAgent = Request.Headers["User-Agent"].FirstOrDefault();
            }

            return userAgent;
        }

        protected string? GetClientIpAddress()
        {
            string? clientIp = null;

            // X-Forwarded-For 헤더 확인 (프록시/로드밸런서 환경)
            var proxyIps = Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(proxyIps))
            {
                // X-Real-IP 헤더 확인 (Nginx 등)
                proxyIps = Request.Headers["X-Real-IP"].FirstOrDefault();

                if (!string.IsNullOrEmpty(proxyIps))
                {
                    var ipList = proxyIps.Split(',').Select(ip => ip.Trim()).ToList();

                    clientIp = ipList.FirstOrDefault();
                }
            }
            else
            {
                var ipList = proxyIps.Split(',').Select(ip => ip.Trim()).ToList();

                clientIp = ipList.FirstOrDefault();
            }

            if (clientIp == "::1")
            {
                clientIp = "127.0.0.1";
            }

            if (string.IsNullOrWhiteSpace(clientIp) == true)
            {
                clientIp = "Unknown";
            }

            // HttpContext의 RemoteIpAddress 사용
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
