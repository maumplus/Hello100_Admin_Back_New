using FluentAssertions;
using Hello100Admin.Modules.Auth.Infrastructure.Persistence;

namespace Hello100Admin.Modules.Auth.Infrastructure.UnitTests.Persistence;

/// <summary>
/// DbConnectionFactory 단위 테스트
/// </summary>
public class DbConnectionFactoryTests
{
    [Fact]
    public void Constructor_ShouldSetConnectionString()
    {
        // Arrange
        var connectionString = "Server=localhost;Database=test;Uid=root;Pwd=pass;";

        // Act
        var factory = new DbConnectionFactory(connectionString);

        // Assert
        factory.Should().NotBeNull();
    }
}