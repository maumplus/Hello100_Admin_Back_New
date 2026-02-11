using System.Net.Http.Json;
using Hello100Admin.Integration.Shared;

namespace Auth.API.IntegrationTests
{
    public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var loginCommand = new
            {
                AccountId = "daemin",
                Password = "qwer1234",
                AppContext = "H02"
            };

            _client.AsSuperAdmin("B81AFBD0", "´ë¹ÎÅ×½ºÆ®");

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task GetMe_ShouldReturnOk_WhenValidCredentials()
        {

            _client.AsSuperAdmin("B81AFBD0", "´ë¹ÎÅ×½ºÆ®");

            // Act
            var response = await _client.GetAsync("/api/auth/me");

            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
