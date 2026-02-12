using Hello100Admin.Integration.Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace AdminUser.API.IntegrationTests
{
    public class KeywordsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public KeywordsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetKeywords_ShouldReturnOk_WhenValidCredentials()
        {
            var query = new Dictionary<string, string?>
            {
                ["keyword"] = default!,
                ["masterSeq"] = default!,
            };

            var url = QueryHelpers.AddQueryString("/api/keywords", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
