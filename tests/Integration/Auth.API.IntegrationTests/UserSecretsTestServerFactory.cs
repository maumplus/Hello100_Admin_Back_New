using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Hello100Admin.API;

namespace Auth.API.IntegrationTests
{
    public class UserSecretsTestServerFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // User Secrets 및 모든 환경설정 자동 포함
                // 별도 추가 필요 없음 (기본적으로 포함됨)
                // 필요시 명시적으로 추가 가능
                // config.AddUserSecrets<Program>();
            });
        }

        public HttpClient CreateTestClient() => this.CreateClient();
    }
}
