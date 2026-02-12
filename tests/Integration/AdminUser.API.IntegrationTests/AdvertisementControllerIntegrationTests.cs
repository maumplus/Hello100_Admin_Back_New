using System.Net.Http.Json;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Integration.Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace AdminUser.API.IntegrationTests
{
    public class AdvertisementControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AdvertisementControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetPopups_ShouldReturnOk_WhenValidCredentials()
        {
            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = 1.ToString(),
                ["PageSize"] = 10.ToString(),
            };

            var url = QueryHelpers.AddQueryString("/api/advertisement/popups", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetPopup_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync("/api/advertisement/popups/145");
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeletePopup_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.DeleteAsync("/api/advertisement/popups/145");
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetEghisBanners_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync("/api/advertisement/eghis-banners");
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BulkUpdateEghisBanners_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new[]
            {
                new { AdId = 131, SortNo = 1, ShowYn = "N" },
                new { AdId = 46, SortNo = 2, ShowYn = "N" },
                new { AdId = 54, SortNo = 3, ShowYn = "N" },
                new { AdId = 161, SortNo = 4, ShowYn = "N" },
                new { AdId = 40, SortNo = 5, ShowYn = "N" },
                new { AdId = 74, SortNo = 6, ShowYn = "N" },
                new { AdId = 48, SortNo = 7, ShowYn = "N" },
                new { AdId = 162, SortNo = 8, ShowYn = "N" },
                new { AdId = 32, SortNo = 9, ShowYn = "N" },
                new { AdId = 31, SortNo = 10, ShowYn = "N" },
                new { AdId = 163, SortNo = 11, ShowYn = "N" },
                new { AdId = 77, SortNo = 12, ShowYn = "N" },
                new { AdId = 112, SortNo = 13, ShowYn = "N" }
            };

            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트", "10350033", "MmM4ZjA4NzJjYmI1YjkxOTAxNzczZmFlOTk0OGYxZmIxZTgyNDEwODhiOWE5MDllNmVkNjk5YTcxOGY0ZjUyNQ==");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/advertisement/eghis-banners/bulk", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
