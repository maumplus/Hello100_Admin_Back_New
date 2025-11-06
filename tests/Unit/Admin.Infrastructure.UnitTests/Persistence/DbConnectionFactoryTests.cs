using FluentAssertions;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence;
using MySqlConnector;

namespace Hello100Admin.Modules.Admin.Infrastructure.UnitTests.Persistence;

/// <summary>
/// DbConnectionFactory 단위 테스트
/// </summary>
public class DbConnectionFactoryTests
{
    [Fact]
    public void CreateConnection_Should_Return_MySqlConnection_With_Correct_ConnectionString()
    {
        // Arrange
        var testConnStr = "Server=localhost;Port=3306;Database=testdb;User=root;Password=test;";
        var factory = new DbConnectionFactory(testConnStr);

        // Act
        using var conn = factory.CreateConnection();

        // Assert
        conn.Should().NotBeNull();
        conn.Should().BeOfType<MySqlConnection>();
        conn.ConnectionString.Should().Be(testConnStr);
    }
}