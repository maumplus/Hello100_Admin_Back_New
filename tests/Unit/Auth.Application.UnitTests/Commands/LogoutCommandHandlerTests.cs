using FluentAssertions;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Logout;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hello100Admin.Modules.Auth.Application.UnitTests.Commands;
public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRevokeSpecificRefreshToken_WhenRefreshTokenProvided()
    {
        // Arrange
        var refreshToken = new RefreshTokenEntity("A0000001", "token123", System.DateTime.UtcNow.AddMinutes(10));
        var authStore = new Mock<IAuthStore>();
        var authRepo = new Mock<IAuthRepository>();
        var mockLogger = new Mock<ILogger<LogoutCommandHandler>>();
        authStore.Setup(r => r.GetByTokenAsync("token123", It.IsAny<CancellationToken>())).ReturnsAsync(refreshToken);
        authRepo.Setup(r => r.UpdateAsync(refreshToken, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var handler = new LogoutCommandHandler(mockLogger.Object, authRepo.Object, authStore.Object);
        var command = new LogoutCommand { Aid = "A0000001" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        refreshToken.IsRevoked.Should().BeTrue();
        authRepo.Verify(r => r.UpdateAsync(refreshToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRevokeAllUserTokens_WhenNoRefreshTokenProvided()
    {
        // Arrange
        var mockRepo = new Mock<IAuthRepository>();
        var mockStore = new Mock<IAuthStore>();
        var mockLogger = new Mock<ILogger<LogoutCommandHandler>>();
        mockRepo.Setup(r => r.RevokeUserTokensAsync("A0000001", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var handler = new LogoutCommandHandler(mockLogger.Object, mockRepo.Object, mockStore.Object);
        var command = new LogoutCommand { Aid = "A0000001" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepo.Verify(r => r.RevokeUserTokensAsync("A0000001", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenRefreshTokenNotFound()
    {
        // Arrange
        var mockStore = new Mock<IAuthStore>();
        var mockRepo = new Mock<IAuthRepository>();
        var mockLogger = new Mock<ILogger<LogoutCommandHandler>>();
        mockStore.Setup(r => r.GetByTokenAsync("notfound", It.IsAny<CancellationToken>())).ReturnsAsync((RefreshTokenEntity?)null);
        var handler = new LogoutCommandHandler(mockLogger.Object, mockRepo.Object, mockStore.Object);
        var command = new LogoutCommand { Aid = "A0000001" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
