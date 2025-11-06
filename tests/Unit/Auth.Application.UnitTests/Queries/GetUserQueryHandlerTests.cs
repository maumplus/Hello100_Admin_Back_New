using FluentAssertions;
using Hello100Admin.Modules.Auth.Application.Queries.GetUser;
using Hello100Admin.Modules.Auth.Domain.Entities;
using Hello100Admin.Modules.Auth.Domain.Interfaces;
using Moq;

namespace Hello100Admin.Modules.Auth.Application.UnitTests.Queries;
public class GetUserQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
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
            Enabled = "1",
            LastLoginDt = DateTime.UtcNow,
            HospNo = "H1234"
        };
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByAidAsync(user.Aid, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var handler = new GetUserQueryHandler(mockRepo.Object);
        var query = new GetUserQuery { UserId = user.Aid };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(user.Aid);
        result.Value.AccountId.Should().Be(user.AccId);
        result.Value.Name.Should().Be(user.Name);
        result.Value.HospNo.Should().Be(user.HospNo);
        result.Value.Grade.Should().Be(user.Grade);
        result.Value.Enabled.Should().BeTrue();
        result.Value.Approved.Should().BeTrue();
        result.Value.AccountLocked.Should().BeTrue();
        result.Value.LastLoginAt.Should().Be(user.LastLoginDt);
        result.Value.Roles.Should().ContainSingle();
        result.Value.Roles[0].Should().Be("SuperAdmin");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByAidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var handler = new GetUserQueryHandler(mockRepo.Object);
        var query = new GetUserQuery { UserId = "NOT_EXIST" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("사용자를 찾을 수 없습니다.");
    }
}
