using Hello100Admin.Integration.Shared;
using Seller.API.IntegrationTests;

namespace AdminUser.API.IntegrationTests
{
    public class ApprovalRequestControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ApprovalRequestControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUntactMedicalRequestsForApproval_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new
            {
                PageNo = 1,
                PageSize = 10,
                ApprYn = "Y"
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync($"/api/approval-request/untact-medical-requests?PageNo=1&PageSize=10&ApprYn=Y");
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetUntactMedicalRequestDetailForApproval_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/approval-request/untact-medical-requests/34");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
