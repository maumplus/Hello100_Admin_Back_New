using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Integration.Shared;
using Microsoft.AspNetCore.WebUtilities;
using Seller.API.IntegrationTests;

namespace AdminUser.API.IntegrationTests
{
    public class HospitalUserControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HospitalUserControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SearchHospitalUsers_ShouldReturnOk_WhenValidCredentials()
        {
            var req = new
            {
                PageNo = 1,
                PageSize = 10,
                FromDate = default(string),
                ToDate = default(string),
                SearchType = 1,
                Keyword = default(string),
            };

            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = req.PageNo.ToString(),
                ["PageSize"] = req.PageSize.ToString(),
                ["FromDate"] = req.FromDate?.ToString(),
                ["ToDate"] = req.ToDate?.ToString(),
                ["KeywordSearchType"] = req.SearchType.ToString(),
                ["SearchKeyword"] = req.Keyword?.ToString()
            };

            var url = QueryHelpers.AddQueryString("/api/hospital-user", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
