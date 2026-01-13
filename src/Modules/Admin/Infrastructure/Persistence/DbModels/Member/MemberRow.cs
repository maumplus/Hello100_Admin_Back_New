namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.Member;

/// <summary>
/// Member DB 모델 (vm_users 뷰 기반)
/// </summary>
internal sealed record MemberRow
{
    public required string Uid { get; init; } // 고객아이디
    public int Mid { get; init; } // 멤버아이디
    public string Name { get; init; } = string.Empty; // 멤버이름
    public string? Pwd { get; init; } // 비밀번호
    public string? SnsId { get; init; } // SNS아이디
    public string? Email { get; init; } // 이메일(암호화)
    public string? Birthday { get; init; } // 생년월일(암호화)
    public string? Sex { get; init; } // 성별
    public string? Phone { get; init; } // 전화번호(암호화)
    public string? Photo { get; init; } // 프로필사진
    public string DelYn { get; init; } = "N"; // 삭제여부
    public string LoginType { get; init; } = "E"; // 로그인타입
    public required string LoginTypeName { get; init; } // 로그인타입명
    public long? Said { get; init; } // 공통등급
    public long RegDt { get; init; } // 등록일시
    public string? RegDtView { get; init; } // 등록일시(뷰)
    public long LastLoginDt { get; init; } // 마지막 로그인 일시
    public string? LastLoginDtView { get; init; } // 마지막 로그인 일시(뷰)
    public string? LastLoginDtViewNew { get; init; } // 마지막 로그인 일시(뷰, 신규포맷???)
    public byte UserRole { get; init; } // 사용자권한(0:일반, 1:테스트계정)
}

/// <summary>
/// MemberFamily DB 모델 (tb_member 테이블)
/// </summary>
internal sealed class MemberFamilyRow
{
    public required string Uid { get; init; } // 고객아이디
    public int Mid { get; init; } // 멤버아이디
    public string Name { get; init; } = string.Empty; // 멤버이름
    public string Birthday { get; init; } = string.Empty; // 생년월일
    public required string Sex { get; init; } // 성별
    public long RegDt { get; init; } // 등록일시
}