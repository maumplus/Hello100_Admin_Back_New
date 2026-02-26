using Hello100Admin.Integration.Shared;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace AdminUser.API.IntegrationTests
{
    public class AssetControllerIntergrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AssetControllerIntergrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task GetUsageList_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new
            {
                PageNo = 1,
                PageSize = 10,
                FromDate = "2024-01-01",
                ToDate = "2026-02-26",
                FromDay = "450",
                ToDay = "460",
                SearchType = 3,
                SearchDateType = 2,
                SearchKeyword = "33"
            };

            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = req.PageNo.ToString(),
                ["PageSize"] = req.PageSize.ToString(),
                ["FromDate"] = req.FromDate?.ToString(),
                ["ToDate"] = req.ToDate?.ToString(),
                ["FromDay"] = req.FromDay?.ToString(),
                ["ToDay"] = req.ToDay?.ToString(),
                ["SearchType"] = req.SearchType.ToString(),
                ["SearchDateType"] = req.SearchDateType.ToString(),
                ["SearchKeyword"] = req.SearchKeyword?.ToString()
            };

            var url = QueryHelpers.AddQueryString("api/asset/usage-list", query);

            _client.AsSuperAdmin("heejin.kwon", "권희진테스트");

            var response = await _client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExportRequestUntactsExcel_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("heejin.kwon", "희진테스트");

            var req = new
            {
                PageNo = 1,
                PageSize = 10,
                FromDate = "2024-01-01",
                ToDate = "2026-02-26",
                FromDay = "450",
                ToDay = "460",
                SearchType = 3,
                SearchDateType = 2,
                SearchKeyword = "33"
            };

            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = req.PageNo.ToString(),
                ["PageSize"] = req.PageSize.ToString(),
                ["FromDate"] = req.FromDate?.ToString(),
                ["ToDate"] = req.ToDate?.ToString(),
                ["FromDay"] = req.FromDay?.ToString(),
                ["ToDay"] = req.ToDay?.ToString(),
                ["SearchType"] = req.SearchType.ToString(),
                ["SearchDateType"] = req.SearchDateType.ToString(),
                ["SearchKeyword"] = req.SearchKeyword?.ToString()
            };

            var url = QueryHelpers.AddQueryString("api/asset/usage-list/export/excel", query);
            var response = await _client.GetAsync(url);
            // Body
            var body = await response.Content.ReadAsStringAsync();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync($"헬로데스크_사용이력_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", bytes);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
