using System.Net.Http.Json;
using Hello100Admin.Integration.Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace Seller.API.IntegrationTests
{
    public class SellerControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SellerControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateSeller_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                HospNo = "10350071",
                BankCd = "BK04",
                DepositNo = "83140201193247",
                Depositor = "이대민2"
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync("/api/seller/add", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetSellerList_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                PageNo = 1,
                PageSize = 20,
                SearchText = default(string),
                IsSync = ""
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync("/api/seller/list", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetSellerDetail_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                Id = 85,
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/seller/{req.Id}");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateSellerRemit_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                HospSellerId = 85,
                Amount = 1000,
                Etc = "TestData",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/seller/remit-add", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateSellerRemit_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                Id = 80,
                Password = "qwer1234",
                Etc = "TestData",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PatchAsJsonAsync($"/api/seller/remit-request", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRemitBalance_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/seller/remit-balance", new { });

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetSellerRemitList_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                PageNo = 1,
                PageSize = 20,
                SearchText = "",
                StartDt = "2025-01-01",
                EndDt = "2026-01-12",
                RemitStatus = "2"
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/seller/remit-list", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetSellerRemitWaitList_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                StartDt = "2025-01-01",
                EndDt = "2026-01-12",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PostAsJsonAsync($"/api/seller/remit-wait-list", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteSellerRemit_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                Id = 80,
                Etc = "TestData",
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PatchAsJsonAsync($"/api/seller/remit-delete", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateSellerRemitEnabled_ShouldReturnOk_WhenValidCredentials()
        {
            // Arrange
            var req = new
            {
                Id = 85,
                Enabled = true,
            };

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.PatchAsJsonAsync($"/api/seller/seller-enable", req);

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// 일단 Bank Api가 하나밖에 없어서 Seller에서 테스트 진행
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetBankList_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            // Act
            var response = await _client.GetAsync($"/api/bank/list");

            // Body
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task GetHospitals_ShouldReturnOk_WhenValidCredentials()
        {
            var query = new Dictionary<string, string?>
            {
                ["searchText"] = "1",
            };

            var url = QueryHelpers.AddQueryString("/api/seller/hospital-list", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
