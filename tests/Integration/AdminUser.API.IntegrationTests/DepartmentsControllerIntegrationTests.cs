using Hello100Admin.Integration.Shared;

namespace AdminUser.API.IntegrationTests
{
    public class DepartmentsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public DepartmentsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetDepartments_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/departments");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
