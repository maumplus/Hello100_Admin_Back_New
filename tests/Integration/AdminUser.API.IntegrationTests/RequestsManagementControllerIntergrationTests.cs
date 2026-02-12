using Hello100Admin.Integration.Shared;
using System.Net.Http.Json;
using Seller.API.IntegrationTests;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Microsoft.AspNetCore.WebUtilities;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;

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
            int hpId = 305;

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
    }
}
