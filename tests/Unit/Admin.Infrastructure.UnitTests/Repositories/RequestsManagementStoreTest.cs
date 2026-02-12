using Hello100Admin.Modules.Admin.Infrastructure.Configuration.Options;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.MySql;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.RequestsManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Admin.Infrastructure.UnitTests.Repositories
{
    public class RequestsManagementStoreTest
    {
        private readonly RequestsManagementStore _store;

        public RequestsManagementStoreTest()
        {
            // DbConnectionOptions 설정 (테스트 DB 연결 문자열)
            var options = Options.Create(new DbConnectionOptions
            {
                Hello100ConnectionString = "Server=172.12.3.98;Port=3306;Database=hello100;User=eghis_adm;Password=dlwltmDB!@1;Charset=utf8;Allow User Variables=True;ConnectionLifeTime=300;Connection Timeout=15;MinimumPoolSize=10;MaximumPoolSize=50;Pooling=True;"
            });

            // MySqlConnectionFactory 생성
            var connectionFactory = new MySqlConnectionFactory(options);

            // Logger 생성 (Console 로깅)
            var logger = NullLogger<RequestsManagementStore>.Instance;

            // Store 생성자에 주입
            _store = new RequestsManagementStore(connectionFactory, logger);

        }

        [Fact]
        public async Task GetRequestBugsAsync_ReturnsPagedResult()
        {
            // Arrange
            //var hospKey = "YTk0YWVmZjUwYTc4YmRhYjRmNGUyNWNiMzFiZTY0ZjIwNDBlYmExYWU1NDkyOGU5ZWNmY2JmNWM1OGU2NDY4Mg==";   // 테스트 DB에 존재하는 병원 키
            var pageSize = 10;
            var pageNum = 1;
            bool apprYn = false;     // 승인 여부 
            var token = CancellationToken.None;

            // Act
            var result = await _store.GetRequestBugsAsync(pageSize, pageNum, apprYn, token);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count <= pageSize);   // 페이징 take 값 확인
            Assert.True(result.TotalCount >= result.Items.Count); // 총 개수와 Items 비교

            // 샘플 데이터 검증 (테스트 DB에 맞게 수정)
            /*
            if (result.Items.Count > 0)
            {
                var first = result.Items.First();
                Assert.False(string.IsNullOrEmpty(first.RequestId)); // RequestId가 비어있지 않은지 확인
            }*/
        }


    }
}
