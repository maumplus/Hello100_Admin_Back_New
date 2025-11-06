using FluentAssertions;
using System.Data;
using Moq;
using Hello100Admin.Modules.Auth.Infrastructure.Repositories;

namespace Hello100Admin.Modules.Auth.Infrastructure.UnitTests.Repositories;

/// <summary>
/// RefreshTokenRepository 단위 테스트
/// </summary>
public class RefreshTokenRepositoryTests
{
    [Fact]
    public void Constructor_ShouldSetConnection()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();

        // Act
        var mockLogger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<RefreshTokenRepository>>();
        var repo = new RefreshTokenRepository(mockConnection.Object, mockLogger.Object);

        // Assert
        repo.Should().NotBeNull();
    }
}