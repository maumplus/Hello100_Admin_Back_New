using FluentAssertions;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Moq;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Login;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Services;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;

namespace Hello100Admin.Modules.Auth.Application.UnitTests.Commands
{
	public class LoginCommandHandlerTests
	{
		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenCredentialsAreValid()
		{
			// Arrange
			var user = new UserEntity
			{
				AId = "A0000001",
				AccId = "testuser",
				AccPwd = "hashedpwd",
				Grade = "S",
				Name = "테스트유저",
				DelYn = "N",
				AccountLocked = "N",
				LoginFailCount = 0,
				Approved = "1",
				Enabled = "1",
				// CreatedAt은 테스트에서 할당하지 않음
				LastLoginDt = DateTime.UtcNow
			};
			var authStore = new Mock<IAuthStore>();
			authStore.Setup(r => r.GetByUsernameAsync(user.AccId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
			var mockPasswordHasher = new Mock<IPasswordHasher>();
			mockPasswordHasher.Setup(h => h.VerifyPassword(user.AccPwd, "password", user.AId)).Returns(true);
			var mockTokenService = new Mock<ITokenService>();
			mockTokenService.Setup(t => t.GenerateAccessToken(user, It.IsAny<string[]>())).Returns("access-token");
			mockTokenService.Setup(t => t.GenerateRefreshToken(user.AId, It.IsAny<string>())).Returns(new Hello100Admin.Modules.Auth.Domain.Entities.RefreshTokenEntity(user.AId, "refresh-token", DateTime.UtcNow.AddMinutes(10)));
			var authRepo = new Mock<IAuthRepository>();
			var mockLogger = new Mock<ILogger<LoginCommandHandler>>();
			var handler = new LoginCommandHandler(mockPasswordHasher.Object, mockTokenService.Object, authRepo.Object, authStore.Object, mockLogger.Object);
			var command = new LoginCommand { AccountId = user.AccId, Password = "password", IpAddress = "127.0.0.1" };

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.Data.Should().NotBeNull();
			result.Data.Token.AccessToken.Should().Be("access-token");
			result.Data.Token.RefreshToken.Should().Be("refresh-token");
			result.Data.User.Id.Should().Be(user.AId);
		}

		[Fact]
		public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
		{
			// Arrange
			var authStore = new Mock<IAuthStore>();
			authStore.Setup(r => r.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((UserEntity?)null);
			var mockPasswordHasher = new Mock<IPasswordHasher>();
			var mockTokenService = new Mock<ITokenService>();
			var authRepo = new Mock<IAuthRepository>();
			var mockLogger = new Mock<ILogger<LoginCommandHandler>>();
			var handler = new LoginCommandHandler(mockPasswordHasher.Object, mockTokenService.Object, authRepo.Object, authStore.Object, mockLogger.Object);
			var command = new LoginCommand { AccountId = "notfound", Password = "password", IpAddress = "127.0.0.1" };

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.ErrorInfo?.Message.Should().Be("계정 ID 또는 비밀번호가 올바르지 않습니다.");
		}

		[Fact]
		public async Task Handle_ShouldReturnFailure_WhenPasswordIsInvalid()
		{
			// Arrange
			var user = new UserEntity
			{
				AId = "A0000001",
				AccId = "testuser",
				AccPwd = "hashedpwd",
				Grade = "S",
				Name = "테스트유저",
				DelYn = "N",
				AccountLocked = "N",
				LoginFailCount = 0,
				Approved = "1",
				Enabled = "1",
				// CreatedAt은 테스트에서 할당하지 않음
				LastLoginDt = DateTime.UtcNow
			};
			var authStore = new Mock<IAuthStore>();
			authStore.Setup(r => r.GetByUsernameAsync(user.AccId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
			var mockPasswordHasher = new Mock<IPasswordHasher>();
			mockPasswordHasher.Setup(h => h.VerifyPassword(user.AccPwd, "wrongpassword", user.AId)).Returns(false);
			var mockTokenService = new Mock<ITokenService>();
			var authRepo = new Mock<IAuthRepository>();
			var mockLogger = new Mock<ILogger<LoginCommandHandler>>();
			var handler = new LoginCommandHandler(mockPasswordHasher.Object, mockTokenService.Object, authRepo.Object, authStore.Object, mockLogger.Object);
			var command = new LoginCommand { AccountId = user.AccId, Password = "wrongpassword", IpAddress = "127.0.0.1" };

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			result.ErrorInfo?.Message.Should().Be("계정 ID 또는 비밀번호가 올바르지 않습니다.");
		}
	}
}
