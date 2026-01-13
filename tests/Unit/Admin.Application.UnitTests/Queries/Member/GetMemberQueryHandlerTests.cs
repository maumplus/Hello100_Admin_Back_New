using FluentAssertions;
using Hello100Admin.Modules.Admin.Application.UnitTests.Builders;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Moq;
using Microsoft.Extensions.Logging;
using Hello100Admin.Modules.Admin.Application.Features.Member.Queries.GetMember;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Member;

namespace Hello100Admin.Modules.Admin.Application.UnitTests.Queries.Member;

public class GetMemberQueryHandlerTests
{
    public GetMemberQueryHandlerTests()
    {
        // EncryptedData 테스트용 FakeCryptoService 등록
        Hello100Admin.BuildingBlocks.Common.Domain.EncryptedData.Configure(new FakeCryptoService());
    }

    private class FakeCryptoService : ICryptoService
    {
        public string Encrypt(string plainText, CryptoKeyType keyType = CryptoKeyType.Default) => plainText;
        public string Decrypt(string encryptedText, CryptoKeyType keyType = CryptoKeyType.Default) => encryptedText;
        public string Encrypt(string plainText) => plainText;
        public string Decrypt(string encryptedText) => encryptedText;
        public string EncryptParameter(string plainText) => plainText;
        public string DecryptParameter(string encryptedText) => encryptedText;
    }

    [Fact]
    public async Task Handle_Returns_Success_When_Member_Exists()
    {
        // Arrange
        var mockRepo = new Mock<IMemberStore>();
        var mockLogger = new Mock<ILogger<GetMemberQueryHandler>>();
        var member = MemberTestData.Default;
        mockRepo.Setup(r => r.GetMember(member.Uid, It.IsAny<CancellationToken>())).ReturnsAsync(member);
        var families = MemberFamilyTestData.DefaultList;
        mockRepo.Setup(r => r.GetMemberFamilys(member.Uid, It.IsAny<CancellationToken>())).ReturnsAsync(families);

        var handler = new GetMemberQueryHandler(mockRepo.Object, mockLogger.Object);
        var query = new GetMemberQuery(member.Uid);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Uid.Should().Be("U0000001");
        result.Data.Name.Should().Be("홍길동");
        // 패밀리 값 검증
        result.Data.MemberFamilys.Should().NotBeNull();
        result.Data.MemberFamilys.Should().ContainSingle();
        var familyDto = result.Data.MemberFamilys[0];
        familyDto.Uid.Should().Be("F0000001");
        familyDto.Mid.Should().Be(2);
        familyDto.Name.Should().Be("가족1");
    }

    [Fact]
    public async Task Handle_Returns_Failure_When_Member_Not_Found()
    {
        // Arrange
        var mockRepo = new Mock<IMemberStore>();
        var mockLogger = new Mock<ILogger<GetMemberQueryHandler>>();
        mockRepo.Setup(r => r.GetMember("U9999999", It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.MemberEntity?)null);
        var handler = new GetMemberQueryHandler(mockRepo.Object, mockLogger.Object);
        var query = new GetMemberQuery("U9999999");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorInfo?.Message.Should().Be("회원이 존재하지 않습니다.");
    }
}
