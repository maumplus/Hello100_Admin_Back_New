using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using Hello100Admin.API;

namespace Auth.API.IntegrationTests
{
    public static class TestServerFactory
    {
        public static HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }
    }
}
