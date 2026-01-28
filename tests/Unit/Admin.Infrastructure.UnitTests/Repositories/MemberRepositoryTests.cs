using FluentAssertions;
using System.Data;
using Dapper;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Microsoft.Data.Sqlite;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.Member;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;

namespace Hello100Admin.Modules.Admin.Infrastructure.UnitTests.Repositories;

public class MemberRepositoryInMemoryTests
{
    private IDbConnection CreateInMemoryConnection()
    {
        var conn = new SqliteConnection("Data Source=:memory:");
        conn.Open();
        // 테이블/뷰 생성 (테스트용)
        conn.Execute(@"
            CREATE TABLE VM_USERS (
                Uid TEXT PRIMARY KEY,
                Name TEXT,
                LoginType TEXT,
                LoginTypeName TEXT,
                RegDt INTEGER,
                UserRole INTEGER,
                LastLoginDt INTEGER
            );
        ");
        return conn;
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Member_When_Found()
    {
        // Arrange
        using var conn = CreateInMemoryConnection();
        // 테스트 데이터 삽입
        conn.Execute("INSERT INTO VM_USERS (Uid, Name, LoginType, LoginTypeName, RegDt, UserRole, LastLoginDt) VALUES (@Uid, @Name, @LoginType, @LoginTypeName, @RegDt, @UserRole, @LastLoginDt)",
            new { Uid = "U0000001", Name = "name", LoginType = "E", LoginTypeName = "이메일", RegDt = 1700000000, UserRole = 0, LastLoginDt = 1700000000 });
        var factory = new TestDbConnectionFactory(conn);
        var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<MemberStore>>().Object;
        var repo = new MemberStore(factory, logger);

        // Act
        var member = await repo.GetByIdAsync("U0000001");

        // Assert
        member.Should().NotBeNull();
        member!.Uid.Should().Be("U0000001");
        member.LoginType.Should().Be("E");
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Null_When_Not_Found()
    {
        // Arrange
        using var conn = CreateInMemoryConnection();
        var factory = new TestDbConnectionFactory(conn);
        var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<MemberStore>>().Object;
        var repo = new MemberStore(factory, logger);

        // Act
        var member = await repo.GetByIdAsync("U9999999");

        // Assert
        member.Should().BeNull();
    }

    // 테스트용 DbConnectionFactory 구현
    private class TestDbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDbConnection _conn;
        public TestDbConnectionFactory(IDbConnection conn) => _conn = conn;
        public IDbConnection CreateConnection() => _conn;
        public IDbConnection CreateDbConnection(DataSource dbType) => _conn;
    }
}
