using System.Net.Http.Json;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Integration.Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace AdminUser.API.IntegrationTests
{
    public class HospitalsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HospitalsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SearchHospitals_ShouldReturnOk_WhenValidCredentials()
        {
            var query = new Dictionary<string, string?>
            {
                ["PageNo"] = 1.ToString(),
                ["PageSize"] = 10.ToString(),
                ["SearchType"] = 0.ToString()
                //["SearchKeyword"] = ""
            };

            var url = QueryHelpers.AddQueryString("/api/hospitals", query);

            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var response = await _client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateHospital_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var req = new
            {
                HospNo = "12345123",
                Name = "테스트 DM병원",
                Addr = "경기도 주소",
                PostCd = "12345",
                Tel = "010-1234-1234",
                Site = "www.naver.com",
                Lat = 123.2,
                Lng = 455.3,
                ChartType = "E",
                IsTest = 0,
                MdCds = new[] { "59", "50" }
            };

            var response = await _client.PostAsJsonAsync("/api/hospitals", req);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetHospitalDetail_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            var req = new
            {
                HospKey = "MmM4ZjA4NzJjYmI1YjkxOTAxNzczZmFlOTk0OGYxZmIxZTgyNDEwODhiOWE5MDllNmVkNjk5YTcxOGY0ZjUyNQ=="
            };

            var response = await _client.PostAsJsonAsync("/api/hospitals/detail", req);
            var body = await response.Content.ReadAsStringAsync();

            var bodyKor = body.FromJson<ApiResponse>();

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        //[Fact]
        //public async Task UpdateNotice_ShouldReturnOk_WhenValidCredentials()
        //{
        //    _client.AsSuperAdmin("B81AFBD0", "대민테스트");

        //    var req = new
        //    {
        //        Title = "테스트 타이틀2",
        //        Content = "테스트 내용2",
        //        SendType = "A",
        //        ShowYn = "N"
        //    };

        //    var response = await _client.PatchAsJsonAsync("/api/notice/notices/968", req);
        //    var body = await response.Content.ReadAsStringAsync();

        //    var bodyKor = body.FromJson<ApiResponse>();

        //    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        //}

        //[Fact]
        //public async Task DeleteNotice_ShouldReturnOk_WhenValidCredentials()
        //{
        //    _client.AsSuperAdmin("B81AFBD0", "대민테스트");

        //    var response = await _client.DeleteAsync("/api/notice/notices/968");
        //    var body = await response.Content.ReadAsStringAsync();

        //    var bodyKor = body.FromJson<ApiResponse>();

        //    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        //}

        [Fact]
        public async Task ExportHospitalsExcel_ShouldReturnOk_WhenValidCredentials()
        {
            _client.AsSuperAdmin("B81AFBD0", "대민테스트");

            //var req = new
            //{
            //    SearchType = 0,
            //    SearchKeyword = "이지스"
            //};

            var response = await _client.GetAsync("/api/hospitals/export/excel?SearchType=0&SearchKeyword=이지스");
            // Body
            var body = await response.Content.ReadAsStringAsync();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync($"병원목록_{DateTime.Now.ToString("yyyyMMdd")}.xlsx", bytes);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
