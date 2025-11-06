using FluentAssertions;
using System;
using Xunit;
using Hello100Admin.Modules.Auth.Application.Services;

namespace Hello100Admin.Modules.Auth.Application.UnitTests.Services
{
    public class PasswordHasherTests
    {
        [Fact]
        public void HashPassword_ShouldReturnDifferentHash_ForDifferentPasswords()
        {
            // Arrange
            var loggerMock = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<PasswordHasher>>();
            var hasher = new PasswordHasher(loggerMock.Object);
            var hash1 = hasher.HashPasswordWithSalt("password1", "A0000001");
            var hash2 = hasher.HashPasswordWithSalt("password2", "A0000001");

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
        {
            // Arrange
            var loggerMock = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<PasswordHasher>>();
            var hasher = new PasswordHasher(loggerMock.Object);
            var password = "TestPassword!";
            var aid = "A0000001";
            var hash = hasher.HashPasswordWithSalt(password, aid);

            // Act
            var result = hasher.VerifyPassword(hash, password, aid);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_ForIncorrectPassword()
        {
            // Arrange
            var loggerMock = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<PasswordHasher>>();
            var hasher = new PasswordHasher(loggerMock.Object);
            var password = "TestPassword!";
            var aid = "A0000001";
            var hash = hasher.HashPasswordWithSalt(password, aid);

            // Act
            var result = hasher.VerifyPassword(hash, "WrongPassword", aid);

            // Assert
            result.Should().BeFalse();
        }
    }
}
