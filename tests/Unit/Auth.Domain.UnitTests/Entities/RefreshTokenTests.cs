using FluentAssertions;
using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Domain.UnitTests.Entities;

public class RefreshTokenTests
{
        private RefreshToken CreateRefreshToken(DateTime? expiresAt = null)
        {
            // Arrange
            return new RefreshToken(
                "A0000001",
                "sample-token",
                expiresAt ?? DateTime.UtcNow.AddMinutes(10),
                "127.0.0.1"
            );
        }

        [Fact]
        public void Constructor_ShouldSetAllProperties_WhenValidArgumentsProvided()
        {
            // Arrange
            var expires = DateTime.UtcNow.AddMinutes(5);

            // Act
            var token = new RefreshToken("AID", "TOKEN", expires, "1.2.3.4");

            // Assert
            token.Aid.Should().Be("AID");
            token.Token.Should().Be("TOKEN");
            token.ExpiresAt.Should().Be(expires);
            token.CreatedByIp.Should().Be("1.2.3.4");
            token.IsRevoked.Should().BeFalse();
        }

        [Fact]
        public void IsExpired_ShouldReturnFalse_WhenTokenIsNotExpired()
        {
            // Arrange
            var token = CreateRefreshToken(DateTime.UtcNow.AddMinutes(5));

            // Act
            var isExpired = token.IsExpired;

            // Assert
            isExpired.Should().BeFalse();
        }

        [Fact]
        public void IsExpired_ShouldReturnTrue_WhenTokenIsExpired()
        {
            // Arrange
            var token = CreateRefreshToken(DateTime.UtcNow.AddMinutes(-1));

            // Act
            var isExpired = token.IsExpired;

            // Assert
            isExpired.Should().BeTrue();
        }

        [Fact]
        public void IsActive_ShouldReturnTrue_WhenTokenIsNotRevokedAndNotExpired()
        {
            // Arrange
            var token = CreateRefreshToken(DateTime.UtcNow.AddMinutes(5));

            // Act
            var isActive = token.IsActive;

            // Assert
            isActive.Should().BeTrue();
        }

        [Fact]
        public void IsActive_ShouldReturnFalse_WhenTokenIsRevoked()
        {
            // Arrange
            var token = CreateRefreshToken(DateTime.UtcNow.AddMinutes(5));
            token.Revoke("1.2.3.4", "new-token");

            // Act
            var isActive = token.IsActive;

            // Assert
            isActive.Should().BeFalse();
        }

        [Fact]
        public void IsActive_ShouldReturnFalse_WhenTokenIsExpired()
        {
            // Arrange
            var token = CreateRefreshToken(DateTime.UtcNow.AddMinutes(-1));

            // Act
            var isActive = token.IsActive;

            // Assert
            isActive.Should().BeFalse();
        }

        [Fact]
        public void Revoke_ShouldSetRevokedProperties_WhenCalled()
        {
            // Arrange
            var token = CreateRefreshToken(DateTime.UtcNow.AddMinutes(5));

            // Act
            token.Revoke("1.2.3.4", "new-token");

            // Assert
            token.IsRevoked.Should().BeTrue();
            token.RevokedAt.Should().NotBeNull();
            token.RevokedByIp.Should().Be("1.2.3.4");
            token.ReplacedByToken.Should().Be("new-token");
        }
}
