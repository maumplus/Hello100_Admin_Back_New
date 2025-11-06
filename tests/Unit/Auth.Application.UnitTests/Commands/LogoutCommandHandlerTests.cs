using FluentAssertions;
using Hello100Admin.Modules.Auth.Application.Commands.Logout;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Hello100Admin.Modules.Auth.Domain.Interfaces;
using Moq;

namespace Hello100Admin.Modules.Auth.Application.UnitTests.Commands;
public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRevokeSpecificRefreshToken_WhenRefreshTokenProvided()
    {
        // Arrange
        var refreshToken = new RefreshToken("A0000001", "token123", System.DateTime.UtcNow.AddMinutes(10));
        var mockRepo = new Mock<IRefreshTokenRepository>();
        mockRepo.Setup(r => r.GetByTokenAsync("token123", It.IsAny<CancellationToken>())).ReturnsAsync(refreshToken);
        mockRepo.Setup(r => r.UpdateAsync(refreshToken, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var handler = new LogoutCommandHandler(mockRepo.Object);
        var command = new LogoutCommand { UserId = "A0000001", RefreshToken = "token123" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        refreshToken.IsRevoked.Should().BeTrue();
        mockRepo.Verify(r => r.UpdateAsync(refreshToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRevokeAllUserTokens_WhenNoRefreshTokenProvided()
    {
        // Arrange
        var mockRepo = new Mock<IRefreshTokenRepository>();
        mockRepo.Setup(r => r.RevokeUserTokensAsync("A0000001", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var handler = new LogoutCommandHandler(mockRepo.Object);
        var command = new LogoutCommand { UserId = "A0000001", RefreshToken = null };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        mockRepo.Verify(r => r.RevokeUserTokensAsync("A0000001", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenRefreshTokenNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IRefreshTokenRepository>();
        mockRepo.Setup(r => r.GetByTokenAsync("notfound", It.IsAny<CancellationToken>())).ReturnsAsync((RefreshToken?)null);
        var handler = new LogoutCommandHandler(mockRepo.Object);
        var command = new LogoutCommand { UserId = "A0000001", RefreshToken = "notfound" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
