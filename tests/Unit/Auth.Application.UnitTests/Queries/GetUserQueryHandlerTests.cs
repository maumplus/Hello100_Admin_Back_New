using FluentAssertions;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Queries.GetUser;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Moq;

namespace Hello100Admin.Modules.Auth.Application.UnitTests.Queries;
public class GetUserQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        var user = new UserEntity
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
            Enabled = "1",
            LastLoginDt = DateTime.UtcNow,
            HospNo = "H1234"
        };
        var mockRepo = new Mock<IAuthStore>();
        mockRepo.Setup(r => r.GetByAidAsync(user.Aid, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var handler = new GetUserQueryHandler(mockRepo.Object);
        var query = new GetUserQuery { UserId = user.Aid };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(user.Aid);
        result.Data.AccountId.Should().Be(user.AccId);
        result.Data.Name.Should().Be(user.Name);
        result.Data.HospNo.Should().Be(user.HospNo);
        result.Data.Grade.Should().Be(user.Grade);
        result.Data.Enabled.Should().BeTrue();
        result.Data.Approved.Should().BeTrue();
        result.Data.AccountLocked.Should().BeTrue();
        result.Data.LastLoginAt.Should().Be(user.LastLoginDt);
        result.Data.Roles.Should().ContainSingle();
        result.Data.Roles[0].Should().Be("SuperAdmin");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IAuthStore>();
        mockRepo.Setup(r => r.GetByAidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((UserEntity?)null);
        var handler = new GetUserQueryHandler(mockRepo.Object);
        var query = new GetUserQuery { UserId = "NOT_EXIST" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorInfo?.Message.Should().Be("사용자를 찾을 수 없습니다.");
    }
}
