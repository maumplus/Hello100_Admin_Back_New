using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Hello100Admin.Modules.Auth.Application.Common.Services;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Features.Auth.ReadModels;

namespace Hello100Admin.Modules.Auth.Application.UnitTests.Services
{
    public class JwtTokenServiceTests
    {
        private JwtTokenService CreateService(
            IConfiguration? config = null,
            IAuthRepository? authRepo = null,
            IAuthStore? authStore = null,
            ILogger<JwtTokenService>? logger = null)
        {
            var configuration = config ?? new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Jwt:SecretKey", "supersecretkey_that_is_long_enough"},
                    {"Jwt:Issuer", "issuer"},
                    {"Jwt:Audience", "audience"},
                    {"Jwt:AccessTokenExpirationMinutes", "30"},
                    {"Jwt:RefreshTokenExpirationDays", "1"}
                }).Build();
            var mockAuthRepository = authRepo ?? new Moq.Mock<IAuthRepository>().Object;
            var mockAuthStore = authStore ?? new Moq.Mock<IAuthStore>().Object;
            var log = logger ?? new Moq.Mock<ILogger<JwtTokenService>>().Object;
            return new JwtTokenService(configuration, mockAuthRepository, mockAuthStore, log);
        }

        [Fact]
        public void GenerateAccessToken_ShouldReturnTokenString()
        {
            // Arrange
            var service = CreateService();
            var user = new AdminModel
            {
                Aid = "A0000001",
                AccId = "testuser",
                AccPwd = "password",
                Grade = "S",
                Name = "테스트유저",
                AccountLocked = "N",
                LoginFailCount = 0
            };

            var roles = new[] { "SuperAdmin" };

            // Act
            var token = service.GenerateAccessToken(user, roles);

            // Assert
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnRefreshTokenObject()
        {
            // Arrange
            var service = CreateService();
            var aid = "A0000001";
            var ip = "127.0.0.1";

            // Act
            var refreshToken = service.GenerateRefreshToken(aid, ip);

            // Assert
            refreshToken.Should().NotBeNull();
            refreshToken.Aid.Should().Be(aid);
            refreshToken.Token.Should().NotBeNullOrEmpty();
            refreshToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnFalse_WhenTokenNotFound()
        {
            // Arrange
            var refreshRepoMock = new Moq.Mock<IAuthStore>();
            refreshRepoMock.Setup(r => r.GetByTokenAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshTokenEntity?)null);
            var service = CreateService(authStore: refreshRepoMock.Object);

            // Act
            var result = await service.ValidateRefreshTokenAsync("invalidtoken");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnFalse_WhenTokenExpired()
        {
            // Arrange
            var expiredToken = new RefreshTokenEntity("A0000001", "token", DateTime.UtcNow.AddDays(-1));
            var authStoreMock = new Moq.Mock<IAuthStore>();
            authStoreMock.Setup(r => r.GetByTokenAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<CancellationToken>()))
                .ReturnsAsync(expiredToken);
            var service = CreateService(authStore: authStoreMock.Object);

            // Act
            var result = await service.ValidateRefreshTokenAsync("expiredtoken");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnTrue_WhenTokenValid()
        {
            // Arrange
            var validToken = new RefreshTokenEntity("A0000001", "token", DateTime.UtcNow.AddDays(1));
            var authStoreMock = new Moq.Mock<IAuthStore>();
            authStoreMock.Setup(r => r.GetByTokenAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<CancellationToken>()))
                .ReturnsAsync(validToken);
            var service = CreateService(authStore: authStoreMock.Object);

            // Act
            var result = await service.ValidateRefreshTokenAsync("validtoken");

            // Assert
            result.Should().BeTrue();
        }
    }
}
