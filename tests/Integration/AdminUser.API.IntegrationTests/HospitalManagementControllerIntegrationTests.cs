using System.Net.Http.Json;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Integration.Shared;
using Microsoft.AspNetCore.WebUtilities;

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
        public async Task GetHospitalAsync_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var req = new { HospNo = "10350072" };

            var response = await _client.PostAsJsonAsync("/api/hospital-management/admin/hospital/detail", req);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task UpsertHospitalAsync_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new { BusinessNo = 1, Description = "테스트용 디스크립션" };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.PostAsJsonAsync("/api/hospital-management/admin/hospital", req);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMyHospital_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync("/api/hospital-management/hospital");
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpsertMyHospital_ShouldReturnOk_WhenValidCredentials()
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
            _client.AsSuperAdmin("B81AFBD0", "대민테스트", "10350033", "MmM4ZjA4NzJjYmI1YjkxOTAxNzczZmFlOTk0OGYxZmIxZTgyNDEwODhiOWE5MDllNmVkNjk5YTcxOGY0ZjUyNQ==");

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
        public async Task GetHelloDeskSetting_ShouldReturnOk_WhenValidCredentials()
        {
            var query = new Dictionary<string, string?>
            {
                ["emplNo"] = default!
            };

            var url = QueryHelpers.AddQueryString("/api/hospital-management/hello-desk-setting", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트", "10350033", "MmM4ZjA4NzJjYmI1YjkxOTAxNzczZmFlOTk0OGYxZmIxZTgyNDEwODhiOWE5MDllNmVkNjk5YTcxOGY0ZjUyNQ==");

            var response = await _client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetDoctorUntactApplicationAsync_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"api/hospital-management/doctor/2/untact-medical-application");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetDoctor_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/hospital-management/doctor/2");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PatchDoctor_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            var req = new
            {
                EmplNo = 1,
                DoctNm = "테스트 의사명",
                DeptCd = "01",
                DeptNm = "테스트 진료과명",
                ViewMinCntYn = "Y",
                ViewMinCnt = "5",
                ViewMinTimeYn = "Y",
                ViewMinTime = "10"
            };

            // Act
            var response = await _client.PatchAsJsonAsync($"/api/hospital-management/doctor", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetSymptomExamKeywordsAsync_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync("/api/hospital-management/hospitals/keywords");
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateSymptomExamKeywordAsync_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var req = new
            {
                MasterName = "테스트 대표 키워드명",
                ShowYn = "Y",
                DetailUseYn = "Y",
                DetailNames = new List<string> { "테스트 상세 키워드명1", "테스트 상세 키워드명2" }
            };

            var response = await _client.PostAsJsonAsync("/api/hospital-management/hospitals/keywords", req);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetSymptomExamKeywordDetailAsync_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync("/api/hospital-management/hospitals/keywords/25");
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateSymptomExamKeywordAsync_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var req = new
            {
                MasterName = "테스트 대표 키워드명 수정 완료",
                ShowYn = "Y",
                DetailUseYn = "Y",
                DetailKeywordItems = new List<object>
                {
                    new { DetailSeq = 78, DetailName = "테스트 상세 키워드명1 완료" }
                    //new { DetailSeq = 0, DetailName = "테스트 상세 키워드명3 신규" }
                }
            };

            var response = await _client.PatchAsJsonAsync("/api/hospital-management/hospitals/keywords/25", req);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
