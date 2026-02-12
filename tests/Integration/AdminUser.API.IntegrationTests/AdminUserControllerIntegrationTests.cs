using Hello100Admin.Integration.Shared;
using System.Net.Http.Json;

namespace AdminUser.API.IntegrationTests
{
    public class AdminUserControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AdminUserControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task UpdatePassword_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                UserId = "daemin",
                NewPassword = "dleoals",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PatchAsJsonAsync($"/api/admin-user/update-password", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
