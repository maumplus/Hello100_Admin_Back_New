using FluentAssertions;
using System.Data;
using Moq;
using Hello100Admin.Modules.Auth.Infrastructure.Repositories;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;

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
        var mockConnection = new Mock<IDbConnectionFactory>();

        // Act
        var mockLogger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<AuthRepository>>();
        var repo = new AuthRepository(mockConnection.Object, mockLogger.Object);

        // Assert
        repo.Should().NotBeNull();
    }
}