using Hello100Admin.Integration.Shared;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;

namespace AdminUser.API.IntegrationTests
{
    public class ServiceUsageControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ServiceUsageControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SearchUntactMedicalHistories_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                PageNo = 1,
                PageSize = 20,
                FromDate = "",
                ToDate = "",
                SearchKeyword = "",
                SearchDateType = "1",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/service-usage/untact-medical/search", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetUntactMedicalPaymentDetail_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            //var req = new
            //{
            //    PaymentId = 150,
            //};
            string paymentId = "150";

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/service-usage/untact-medical/payments/{paymentId}");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExportUntactMedicalHistorysExcel_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                SearchDateType = "1",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/service-usage/untact-medical/export/excel", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync($"비대면진료내역_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", bytes);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SearchExaminationResultAlimtalkHistories_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                PageNo = 1,
                PageSize = 20,
                FromDate = "2025-10-01",
                ToDate = "2025-10-31",
                DateRangeType = "1",
                SearchDateType = "1",
                SearchKeyword = "",
                SendStatus = "0",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/service-usage/examination-results/alimtalk/histories/search", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExportExaminationResultAlimtalkHistoriesExcel_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                FromDate = "2025-09-01",
                ToDate = "2026-01-01",
                DateRangeType = "1",
                SearchDateType = "1",
                SearchKeyword = "",
                SendStatus = "0",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/service-usage/examination-results/alimtalk/histories/export/excel", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync($"진단검사결과 알림톡 발송 내역_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", bytes);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRegistrationAlimtalkApplicationInfo_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/service-usage/alimtalk-service/registration/application-info");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetExaminationResultAlimtalkApplicationInfo_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/service-usage/alimtalk-service/examination-results/application-info");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SubmitAlimtalkApplicationRequest_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                DoctNm = "1",
                DoctTel = "1",
                TmpType = "Test",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/service-usage/alimtalk-service/submit", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetHospitalServiceUsageStatus_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = 1.ToString(),
                ["PageSize"] = 10.ToString(),
                ["FromDate"] = "2024-01-01",
                ["ToDate"] = "2026-02-13",
                ["SearchKeyword"] = "10350011",
                ["SearchType"] = 2.ToString(),
                ["QrCheckInYn"] = "Y",
                ["TodayRegistrationYn"] = "Y",
                ["AppointmentYn"] = "Y",
                ["TelemedicineYn"] = "Y",
                ["ExcludeTestHospitalsYn"] = "Y",
            };

            var url = QueryHelpers.AddQueryString("/api/service-usage/hospitals", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync(url);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExportServiceUnitReceptionStatusExcel_ShouldReturnOk_WhenValidCredentials()
        {
            var query = new Dictionary<string, string?>
            {
                ["FromDate"] = "2024-01-01",
                ["ToDate"] = "2026-02-13",
                ["SearchKeyword"] = "TESTDM",
                ["SearchType"] = 1.ToString(),
                ["QrCheckInYn"] = "Y",
                ["TodayRegistrationYn"] = "Y",
                ["AppointmentYn"] = "Y",
                ["TelemedicineYn"] = "Y",
                ["ExcludeTestHospitalsYn"] = "Y"
            };

            var url = QueryHelpers.AddQueryString("/api/service-usage/receptions/excel/by-service-unit", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync(url);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync($"병원별서비스이용현황(상단)_{DateTime.Now:yyyyMMdd}.xlsx", bytes);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExportHospitalUnitReceptionStatusExcel_ShouldReturnOk_WhenValidCredentials()
        {
            var query = new Dictionary<string, string?>
            {
                ["FromDate"] = "2024-01-01",
                ["ToDate"] = "2026-02-13",
                ["SearchKeyword"] = "",
                ["SearchType"] = 1.ToString(),
                ["QrCheckInYn"] = "Y",
                ["TodayRegistrationYn"] = "Y",
                ["AppointmentYn"] = "Y",
                ["TelemedicineYn"] = "Y",
                ["ExcludeTestHospitalsYn"] = "Y"
            };

            var url = QueryHelpers.AddQueryString("/api/service-usage/receptions/excel/by-hospital-unit", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync(url);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync($"병원별서비스이용현황(하단)_{DateTime.Now:yyyyMMdd}.xlsx", bytes);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExportHello100ReceptionStatusExcel_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync("/api/service-usage/receptions/excel/by-hello100?fromDate=2024-01-01&toDate=2026-02-13");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync($"헬로100접수현황_{DateTime.Now:yyyyMMdd}.xlsx", bytes);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
