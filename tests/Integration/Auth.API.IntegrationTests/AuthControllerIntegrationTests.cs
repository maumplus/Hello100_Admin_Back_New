using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Auth.API.IntegrationTests
{
    public class AuthControllerIntegrationTests
    {
        private readonly HttpClient _client;
        public AuthControllerIntegrationTests()
        {
            _client = new UserSecretsTestServerFactory().CreateTestClient();
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var loginCommand = new
            {
                AccountId = "testst",
                Password = "testst"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
