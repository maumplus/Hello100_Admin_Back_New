using FluentAssertions;
using Moq;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Auth.Infrastructure.Repositories;

namespace Hello100Admin.Modules.Auth.Infrastructure.UnitTests.Repositories;

/// <summary>
/// UserRepository 단위 테스트
/// </summary>
public class UserRepositoryTests
{
    [Fact]
    public void Constructor_ShouldSetConnectionFactory()
    {
        // Arrange
        var mockFactory = new Mock<IDbConnectionFactory>();

        // Act
        var mockLogger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<AuthStore>>();
        var repo = new AuthStore(mockFactory.Object, mockLogger.Object);

        // Assert
        repo.Should().NotBeNull();
    }
}