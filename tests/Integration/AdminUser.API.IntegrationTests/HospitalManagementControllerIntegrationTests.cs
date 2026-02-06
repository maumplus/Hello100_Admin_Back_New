using System.Net.Http.Json;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Integration.Shared;
using Microsoft.AspNetCore.WebUtilities;
using Seller.API.IntegrationTests;

namespace AdminUser.API.IntegrationTests
{
    public class HospitalManagementControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HospitalManagementControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetHospital_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync("/api/hospital-management/hospital");
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpsertHospital_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new { BusinessNo = 1, Description = "테스트용 디스크립션" };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.PostAsJsonAsync("/api/hospital-management/hospital", req);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetHello100Setting_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/hospital-management/hello100-setting");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpsertHello100Setting_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                NoticeId = 943,
                StId = 1046,
                Roles = new List<int> { 1, 2, 16, 32 },
                AwaitRole = 1,
                ReceptEndTime = "",
                Notice = "테스트용 공지사항입니다.",
                ExamPushSet = 2
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/hospital-management/hello100-setting", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMedicalDepartments_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/hospital-management/medical-departments");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetClinicalKeywords_ShouldReturnOk_WhenValidCredentials()
        {
            var query = new Dictionary<string, string?>
            {
                ["keyword"] = default!,
                ["masterSeq"] = default!,
            };

            var url = QueryHelpers.AddQueryString("/api/hospital-management/clinical-keywords", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
