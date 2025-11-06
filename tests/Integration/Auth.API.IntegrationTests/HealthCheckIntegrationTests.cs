using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Auth.API.IntegrationTests
{
    public class HealthCheckIntegrationTests : IClassFixture<UserSecretsTestServerFactory>
    {
        private readonly HttpClient _client;

        public HealthCheckIntegrationTests(UserSecretsTestServerFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("/health")]
        [InlineData("/health/ready")]
        public async Task HealthCheck_Endpoints_ReturnHealthy(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content);
        }
    }
}
