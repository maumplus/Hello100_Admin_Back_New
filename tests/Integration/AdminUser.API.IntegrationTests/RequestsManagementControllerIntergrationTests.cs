using Hello100Admin.Integration.Shared;
using System.Net.Http.Json;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.API.Constracts.Admin.RequestsManagement;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace AdminUser.API.IntegrationTests
{
    public class RequestsManagementControllerIntergrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public RequestsManagementControllerIntergrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetRequestBugs_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new
            {
                PageNo = 1,
                PageSize = 10,
                ApprYn = false
            };

            _client.AsSuperAdmin("heejin.kwon", "희진테스트");

            var response = await _client.GetAsync($"api/requests-management/bugs?PageNo={req.PageNo}&PageSize={req.PageSize}&ApprYn={req.ApprYn}");
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRequestBug_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new
            {
                HpId = 305
            };

            _client.AsSuperAdmin("heejin.kwon", "희진테스트");

            var response = await _client.GetAsync($"api/requests-management/bugs/{req.HpId}?HpId={req.HpId}");
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRequestBug_ShouldReturnOk_WhenValidCredentials()
        {
            int hpId = 400;

            var req = new
            {
                ApprAid = "8E54B863"
            };

            _client.AsSuperAdmin("heejin.kwon", "희진테스트");

            var response = await _client.PatchAsJsonAsync($"api/requests-management/bugs/{hpId}", req);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRequestUntacts_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new
            {
                PageNo = 1,
                PageSize = 10,
                FromDate = default(string),
                ToDate = default(string),
                SearchType = 1,
                SearchDateType = 1,
                SearchKeyword = default(string),
                JoinState = new [] { "01", "02", "03"}
            };

            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = req.PageNo.ToString(),
                ["PageSize"] = req.PageSize.ToString(),
                ["FromDate"] = req.FromDate?.ToString(),
                ["ToDate"] = req.ToDate?.ToString(),
                ["SearchType"] = req.SearchType.ToString(),
                ["SearchDateType"] = req.SearchType.ToString(),
                ["SearchKeyword"] = req.SearchKeyword?.ToString()
            };

            int idx = 0;
            foreach (var item in req.JoinState)
            {
                query.Add($"JoinState[{idx}]", item);
                idx++;
            }

            var url = QueryHelpers.AddQueryString("/api/requests-management/untacts", query);

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
                FromDate = default(string),
                ToDate = default(string),
                SearchType = 1,
                SearchDateType = 1,
                SearchKeyword = default(string),
                JoinState = new[] { "01", "02", "03" }
            };

            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = req.PageNo.ToString(),
                ["PageSize"] = req.PageSize.ToString(),
                ["FromDate"] = req.FromDate?.ToString(),
                ["ToDate"] = req.ToDate?.ToString(),
                ["SearchType"] = req.SearchType.ToString(),
                ["SearchDateType"] = req.SearchType.ToString(),
                ["SearchKeyword"] = req.SearchKeyword?.ToString()
            };

            int idx = 0;
            foreach (var item in req.JoinState)
            {
                query.Add($"JoinState[{idx}]", item);
                idx++;
            }

            var url = QueryHelpers.AddQueryString("/api/requests-management/untacts/export/excel", query);
            var response = await _client.GetAsync(url);
            // Body
            var body = await response.Content.ReadAsStringAsync();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync($"비대면진료 신청목록_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", bytes);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRequestUntact_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new
            {
                seq = 42
            };

            _client.AsSuperAdmin("heejin.kwon", "희진테스트");

            var response = await _client.GetAsync($"api/requests-management/untacts/{req.seq}?seq={req.seq}");
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRequestUntact_ShouldReturnOk_WhenValidCredentials()
        {
            int seq = 42;

            var req = new
            {
                JoinState = "03",
                ReturnReason = "반려합니다."
            };

            _client.AsSuperAdmin("heejin.kwon", "희진테스트");

            var response = await _client.PatchAsJsonAsync($"api/requests-management/untacts/{seq}", req);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
