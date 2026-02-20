using System.Net.Http.Json;
using Hello100Admin.Integration.Shared;

namespace AdminUser.API.IntegrationTests
{
    public class VisitPurposeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public VisitPurposeControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetVisitPurposes_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/visit-purpose/visit-purposes");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetVisitPurposeDetail_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트", "10350072", "MzNiMDI0OWI1MDA0MWU2YjcxNTE1YTQ2MDM0YWViYzVmYzY5ZGQ5YjY2M2I5ODBjODgwNDVhNDg3YTdkNzc4NA==");

            // Act
            var response = await _client.GetAsync($"/api/visit-purpose/visit-purposes/01");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BulkUpdateVisitPurposes_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange


            var req = new[]
            {
                new { SortNo = 2, VpCd = "01", ShowYn = "N" },
                new { SortNo = 1, VpCd = "02", ShowYn = "N" }
            };

            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/visit-purpose/visit-purposes/bulk", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateVisitPurpose_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new
            {
                PaperYn = "Y",
                DetailYn = "Y",
                Details = new List<string> { "하이1", "하이2", "하이3" },
                InquiryIdx = 9,
                Name = "진료과목",
                ShowYn = "Y",
                SortNo = 0,
                Roles = new List<int> { 1, 4 },
                DelYn = default(string)
            };

            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.PostAsJsonAsync($"/api/visit-purpose/visit-purposes/add", req);
            var body = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateVisitPurposeForNhisHealthScreening_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange

            var req = new
            {
                ShowYn = "Y",
                Roles = new List<int> { 1, 4 },
                DetailShowYn = new List<string> { "0101", "0102" }
            };

            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/visit-purpose/visit-purposes/nhis-health-screening", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateVisitPurposeForNonNhisHealthScreening_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange

            //var req = new
            //{
            //    PaperYn = "Y",
            //    DetailYn = "Y",
            //    Details = new List<string> { "유방", "갑상선", "가슴성형,유두성형,부유방,여유증", "피부 혹 ", "면역수액치료", "기타증상" },
            //    VpCd = "02",
            //    ParentCd = default(string),
            //    HospKey = default(string),
            //    InquiryIdx = 9,
            //    InpuirySkipYn = default(string),
            //    Name = "진료과목",
            //    ShowYn = "Y",
            //    SortNo = 0,
            //    Roles = new List<int> { 1, 4 },
            //    DelYn = default(string)
            //};

            var req = new
            {
                VpCd = "06",
                Name = "진료과목",
                PaperYn = "N",
                DetailYn = "N",
                ShowYn = "N",
                InquiryIdx = -1,
                Details = new List<string> { "하이1", "하이2", "하이3" },
                Roles = new List<int> { },
            };

            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/visit-purpose/visit-purposes/non-nhis-health-screening", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteVisitPurpose_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PatchAsJsonAsync($"/api/visit-purpose/visit-purposes/06", "06");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCertificates_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/visit-purpose/certificates");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BulkUpdateCertificates_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new[]
            {
                new { ReDocCd = "01", ShowYn = "Y", SortNo = 3 },
                new { ReDocCd = "02", ShowYn = "Y", SortNo = 1 },
                new { ReDocCd = "03", ShowYn = "Y", SortNo = 2 },
                new { ReDocCd = "04", ShowYn = "Y", SortNo = 4 }
            };

            _client.AsMySuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.PutAsJsonAsync($"/api/visit-purpose/certificates/bulk", req);
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
