using Hello100Admin.BuildingBlocks.Common.Domain;

namespace Hello100Admin.Modules.Admin.Domain.Entities;

/// <summary>
/// 회원정보 엔티티 (tb_user)
/// </summary>
public class Member : AggregateRoot<string>
{
    /// <summary>
    /// 고객아이디 
    /// </summary>
    public required string Uid { get; set; } // varchar(8)

    /// <summary>
    /// 멤버 아이디 
    /// </summary>
    public int Mid { get; set; } // int

    /// <summary>
    /// 이름 (암호화)
    /// </summary>
    public required EncryptedData Name { get; set; } // varchar(128)

    /// <summary>
    /// SNS아이디(카카오,네이버,페이스북,이메일) (암호화)
    /// </summary>
    public EncryptedData? SnsId { get; set; } // varchar(128)

    /// <summary>
    /// 이메일 (암호화)
    /// </summary>
    public EncryptedData? Email { get; set; } // varchar(128)

    /// <summary>
    /// 전화번호 (암호화)
    /// </summary>
    public EncryptedData? Phone { get; set; } // varchar(50)

    /// <summary>
    /// 로그인타입(tb_common테이블 cls_cd 01참고)
    /// </summary>
    public required string LoginType { get; set; } // char(1)

    /// <summary>
    /// 로그인타입명
    /// </summary>
    public required string LoginTypeName { get; set; } // varchar(50)

    /// <summary>
    /// 본인인증아이디
    /// </summary>
    public long? Said { get; set; }

    /// <summary>
    /// 등록일시 뷰
    /// </summary>
    public required string RegDtView { get; set; }

    /// <summary>
    /// 마지막 로그인 일시(Unix Timestamp)
    /// </summary>
    public DateTime LastLoginDt { get; set; }

    /// <summary>
    /// 마지막 로그인 일시 뷰
    /// </summary>
    public string? LastLoginDtView { get; set; }

    /// <summary>
    /// 마지막 로그인 일시 뷰(신규포맷)
    /// </summary>
    public string? LastLoginDtViewNew { get; set; }

    /// <summary>
    /// 사용자권한(0:일반, 1:테스트계정)
    /// </summary>
    public byte UserRole { get; set; }

    private readonly List<MemberFamily> _members = new();

    /// <summary>
    /// 고객의 멤버 목록 (가족 구성원 등)
    /// </summary>
    public IReadOnlyCollection<MemberFamily> Members => _members.AsReadOnly();

    /// <summary>
    /// 멤버 컬렉션을 주입하는 팩토리 메서드
    /// </summary>
    public Member WithMembers(IEnumerable<MemberFamily> members)
    {
        _members.Clear();
        _members.AddRange(members);
        return this;
    }
}

/// <summary>
/// 멤버정보 엔티티 (tb_member)
/// </summary>
public class MemberFamily : AggregateRoot<int>
{
    /// <summary>
    /// 멤버아이디 (PK, AUTO_INCREMENT)
    /// </summary>
    public int Mid { get; set; }

    /// <summary>
    /// 회원아이디 (FK)
    /// </summary>
    public required string Uid { get; set; } // varchar(8)

    /// <summary>
    /// 멤버이름 (암호화)
    /// </summary>
    public required EncryptedData Name { get; set; } // varchar(128)

    /// <summary>
    /// 생년월일 (암호화)
    /// </summary>
    public required EncryptedData Birthday { get; set; } // varchar(50)

    /// <summary>
    /// 성별 (암호화)
    /// </summary>
    public required EncryptedData Sex { get; set; } // varchar(50)
}