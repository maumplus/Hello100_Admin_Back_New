using FluentAssertions;
using Hello100Admin.BuildingBlocks.Common.Domain;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Domain.Entities;

namespace Hello100Admin.Modules.Admin.Domain.UnitTests.Entities;

public class MemberTests
{
    [Fact]
    public void Member_Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var member = new MemberEntity
        {
            Uid = "U0000001",
            Mid = 123,
            Name = EncryptedData.FromEncrypted("encryptedName", CryptoKeyType.Name),
            SnsId = EncryptedData.FromEncrypted("encryptedSnsId"),
            Email = EncryptedData.FromEncrypted("encryptedEmail", CryptoKeyType.Email),
            Phone = EncryptedData.FromEncrypted("encryptedPhone"),
            LoginType = "E",
            LoginTypeName = "이메일",
            Said = 456789,
            RegDtView = "2025-10-29 10:00:00",
            LastLoginDt = new DateTime(2025, 10, 29, 10, 0, 0),
            LastLoginDtView = "2025-10-29 10:00:00",
            LastLoginDtViewNew = "2025-10-29T10:00:00Z",
            UserRole = 1
        };

        // Assert
        member.Uid.Should().Be("U0000001");
        member.Mid.Should().Be(123);
        member.Name.EncryptedValue.Should().Be("encryptedName");
        member.SnsId?.EncryptedValue.Should().Be("encryptedSnsId");
        member.Email?.EncryptedValue.Should().Be("encryptedEmail");
        member.Phone?.EncryptedValue.Should().Be("encryptedPhone");
        member.LoginType.Should().Be("E");
        member.LoginTypeName.Should().Be("이메일");
        member.Said.Should().Be(456789);
        member.RegDtView.Should().Be("2025-10-29 10:00:00");
        member.LastLoginDt.Should().Be(new DateTime(2025, 10, 29, 10, 0, 0));
        member.LastLoginDtView.Should().Be("2025-10-29 10:00:00");
        member.LastLoginDtViewNew.Should().Be("2025-10-29T10:00:00Z");
        member.UserRole.Should().Be((byte)1);
    }

    [Fact]
    public void Member_WithMembers_Should_Set_Members_Collection()
    {
        // Arrange
        var member = new MemberEntity
        {
            Uid = "U0000001",
            Name = EncryptedData.FromEncrypted("encryptedName"),
            LoginType = "E",
            LoginTypeName = "이메일",
            RegDtView = "2025-10-29 10:00:00"
        };
        var family1 = new MemberFamilyEntity
        {
            Mid = 1,
            Uid = "U0000001",
            Name = EncryptedData.FromEncrypted("fam1"),
            Birthday = EncryptedData.FromEncrypted("19900101"),
            Sex = EncryptedData.FromEncrypted("M")
        };
        var family2 = new MemberFamilyEntity
        {
            Mid = 2,
            Uid = "U0000001",
            Name = EncryptedData.FromEncrypted("fam2"),
            Birthday = EncryptedData.FromEncrypted("19950101"),
            Sex = EncryptedData.FromEncrypted("F")
        };
        var families = new List<MemberFamilyEntity> { family1, family2 };

        // Act
        member.WithMembers(families);

        // Assert
        member.Members.Should().HaveCount(2);
        member.Members.Should().Contain(m => m.Mid == 1 && m.Name.EncryptedValue == "fam1");
        member.Members.Should().Contain(m => m.Mid == 2 && m.Name.EncryptedValue == "fam2");
    }
}