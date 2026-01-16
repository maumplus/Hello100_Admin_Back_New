using Hello100Admin.Integration.Shared;
using System.Net.Http.Json;
using Seller.API.IntegrationTests;

namespace AdminUser.API.IntegrationTests
{
    public class ServiceUsageControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ServiceUsageControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUntactMedicalHistory_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                PageNo = 1,
                PageSize = 20,
                FromDate = "",
                ToDate = "",
                SearchKeyword = "",
                SearchDateType = "1",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/serviceusage/list", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
