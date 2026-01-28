using Hello100Admin.Integration.Shared;
using Seller.API.IntegrationTests;

namespace AdminUser.API.IntegrationTests
{
    public class HospitalStatisticsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HospitalStatisticsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetRegistrationStatsByMethod_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync($"/api/hospitalstatistics/me/patient-statistics/registrations/by-method/2025");
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRegistrationStatusSummary_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/hospitalstatistics/me/patient-statistics/registrations/status-summary/2025");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRegistrationStatsByVisitPurpose_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/hospitalstatistics/me/patient-statistics/registrations/by-visit-purpose/2025");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
