using System.Security.Claims;
using System.Text.Encodings.Web;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hello100Admin.Integration.Shared
{
    public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Test";
        public const string HeaderName = "X-Test-Auth";
        // 예: "sub=1;name=kim;role=Admin"

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
        : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderName, out var headerValue))
                return Task.FromResult(AuthenticateResult.Fail("Missing test auth header."));

            var raw = headerValue.ToString();

            var claims = new List<Claim>
            {
                //new Claim(ClaimTypes.NameIdentifier, "test-user"),
                //new Claim(ClaimTypes.Name, "test-user"),
            };

            // 필요 시 더 확장
            foreach (var part in raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var kv = part.Split('=', 2, StringSplitOptions.TrimEntries);
                if (kv.Length != 2) continue;

                var key = kv[0];
                var value = kv[1];

                switch (key.ToLowerInvariant())
                {
                    case "sub":
                        claims.Add(new Claim("sub", value));
                        break;

                    case "account_id":
                        claims.Add(new Claim("account_id", value));
                        break;

                    case "name":
                        claims.Add(new Claim(ClaimTypes.Name, value));
                        break;

                    case "role":
                        claims.Add(new Claim(ClaimTypes.Role, value));
                        break;

                    case "hospital_number":
                        claims.Add(new Claim("hospital_number", value));
                        break;

                    case "hospital_key":
                        claims.Add(new Claim("hospital_key", value));
                        break;

                    default:
                        claims.Add(new Claim(key, value));
                        break;
                }
            }

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
