using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Auth.Domain.Interfaces;
using Moq;
using Hello100Admin.Modules.Auth.Application.Services;
using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Application.UnitTests.Services
{
    public class JwtTokenServiceTests
    {
        private JwtTokenService CreateService(
            IConfiguration? config = null,
            IRefreshTokenRepository? refreshRepo = null,
            IUserRepository? userRepo = null,
            ILogger<JwtTokenService>? logger = null)
        {
            var configuration = config ?? new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Jwt:SecretKey", "supersecretkey_that_is_long_enough"},
                    {"Jwt:Issuer", "issuer"},
                    {"Jwt:Audience", "audience"},
                    {"Jwt:AccessTokenExpirationMinutes", "10"},
                    {"Jwt:RefreshTokenExpirationDays", "7"}
                }).Build();
            var refreshTokenRepo = refreshRepo ?? new Moq.Mock<IRefreshTokenRepository>().Object;
            var userRepository = userRepo ?? new Moq.Mock<IUserRepository>().Object;
            var log = logger ?? new Moq.Mock<ILogger<JwtTokenService>>().Object;
            return new JwtTokenService(configuration, refreshTokenRepo, userRepository, log);
        }

        [Fact]
        public void GenerateAccessToken_ShouldReturnTokenString()
        {
            // Arrange
            var service = CreateService();
            var user = new User
            {
                Aid = "A0000001",
                AccId = "testuser",
                AccPwd = "password",
                Grade = "S",
                Name = "테스트유저",
                DelYn = "N",
                AccountLocked = "0",
                LoginFailCount = 0,
                Approved = "1",
                Enabled = "1"
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
            var refreshRepoMock = new Moq.Mock<IRefreshTokenRepository>();
            refreshRepoMock.Setup(r => r.GetByTokenAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshToken?)null);
            var service = CreateService(refreshRepo: refreshRepoMock.Object);

            // Act
            var result = await service.ValidateRefreshTokenAsync("invalidtoken");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnFalse_WhenTokenExpired()
        {
            // Arrange
            var expiredToken = new RefreshToken("A0000001", "token", DateTime.UtcNow.AddDays(-1));
            var refreshRepoMock = new Moq.Mock<IRefreshTokenRepository>();
            refreshRepoMock.Setup(r => r.GetByTokenAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<CancellationToken>()))
                .ReturnsAsync(expiredToken);
            var service = CreateService(refreshRepo: refreshRepoMock.Object);

            // Act
            var result = await service.ValidateRefreshTokenAsync("expiredtoken");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnTrue_WhenTokenValid()
        {
            // Arrange
            var validToken = new RefreshToken("A0000001", "token", DateTime.UtcNow.AddDays(1));
            var refreshRepoMock = new Moq.Mock<IRefreshTokenRepository>();
            refreshRepoMock.Setup(r => r.GetByTokenAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<CancellationToken>()))
                .ReturnsAsync(validToken);
            var service = CreateService(refreshRepo: refreshRepoMock.Object);

            // Act
            var result = await service.ValidateRefreshTokenAsync("validtoken");

            // Assert
            result.Should().BeTrue();
        }
    }
}
