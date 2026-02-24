using Hello100Admin.Integration.Shared;
using Microsoft.AspNetCore.WebUtilities;
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
        public async Task GetHospitalsUsingHello100ServiceAsync_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = "1",
                ["PageSize"] = "10",
                ["SearchChartType"] = "",
                ["SearchType"] = "1"
                //["SearchKeyword"] = "",
            };

            var url = QueryHelpers.AddQueryString("/api/hospital-management/hospitals/hello100-service", query);

            // Act
            var response = await _client.GetAsync(url);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
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

        [Fact]
        public async Task GetAdminUsersAsync_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"api/admin-user/list?pageNo=2&pageSize=10");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetHospitalAdminListAsync_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = "1",
                ["PageSize"] = "10",
                ["QrState"] = "0",
                ["SearchType"] = "1",
                ["SearchKeyword"] = "",
            };

            var url = QueryHelpers.AddQueryString("api/admin-user/hospital-admins", query);

            // Act
            var response = await _client.GetAsync(url);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetHospitalAdminDetailAsync_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            var req = new
            {
                AId = "6DCBDB04",
            };

            // Act
            var response = await _client.PostAsJsonAsync($"api/admin-user/hospital-admins/detail", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteHospitalAdminMappingAsync_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            var query = new Dictionary<string, string?>
            {
                ["HospitalAId"] = "6DCBE0E2",
                ["AccPwd"] = "qwer1234",
            };

            var url = QueryHelpers.AddQueryString("/api/admin-user/hospital-admins/mapping", query);

            // Act
            var response = await _client.DeleteAsync(url);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
