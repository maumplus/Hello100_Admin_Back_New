using FluentAssertions;
using Hello100Admin.Modules.Auth.Domain.Entities;

namespace Hello100Admin.Modules.Auth.Domain.UnitTests.Entities;

/// <summary>
/// User 엔티티 단위 테스트
/// </summary>
public class UserTests
{
    private User CreateDefaultUser()
    {
        return new User
        {
            Aid = "A0000001",
            AccId = "testuser",
            AccPwd = "password",
            Grade = "A",
            Name = "테스트유저",
            DelYn = "N",
            AccountLocked = "0",
            LoginFailCount = 0,
            Approved = "0",
            Enabled = "1"
        };
    }

    [Fact]
    public void CanLogin_ShouldReturnTrue_WhenAllLoginConditionsAreMet()
    {
        // Arrange
        var user = CreateDefaultUser();

        // Act
        var canLogin = user.CanLogin();

        // Assert
        canLogin.Should().BeTrue();
    }

    [Theory]
    [InlineData("Y", "0", "1", "0")]
    [InlineData("N", "1", "1", "0")]
    [InlineData("N", "0", "0", "0")]
    [InlineData("N", "0", "1", "1")]
    public void CanLogin_ShouldReturnFalse_WhenAnyLoginConditionIsNotMet(string delYn, string approved, string enabled, string accountLocked)
    {
        // Arrange
        var user = CreateDefaultUser();
        user.DelYn = delYn;
        user.Approved = approved;
        user.Enabled = enabled;
        user.AccountLocked = accountLocked;

        // Act
        var canLogin = user.CanLogin();

        // Assert
        canLogin.Should().BeFalse();
    }

    [Fact]
    public void RecordLogin_ShouldResetLoginFailCountAndLockAccountAndUpdateLastLoginAt()
    {
        // Arrange
        var user = CreateDefaultUser();
        user.LoginFailCount = 3;
        user.AccountLocked = "0";
        var before = user.LastLoginDt;

        // Act
        user.RecordLogin();

        // Assert
        user.LoginFailCount.Should().Be(0);
        user.AccountLocked.Should().Be("1");
        (user.LastLoginDt > before || user.LastLoginDt != null).Should().BeTrue();
    }

    [Fact]
    public void RecordLoginFailure_ShouldIncrementLoginFailCount_WhenLoginFails()
    {
        // Arrange
        var user = CreateDefaultUser();
        user.LoginFailCount = 2;
        user.AccountLocked = "0";

        // Act
        user.RecordLoginFailure();

        // Assert
        user.LoginFailCount.Should().Be(3);
        user.AccountLocked.Should().Be("0");
    }

    [Fact]
    public void RecordLoginFailure_ShouldLockAccount_WhenLoginFailCountReachesFive()
    {
        // Arrange
        var user = CreateDefaultUser();
        user.LoginFailCount = 4;
        user.AccountLocked = "0";

        // Act
        user.RecordLoginFailure();

        // Assert
        user.LoginFailCount.Should().Be(5);
        user.AccountLocked.Should().Be("1");
    }
}