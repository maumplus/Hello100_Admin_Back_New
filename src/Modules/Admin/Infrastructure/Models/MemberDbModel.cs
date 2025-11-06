namespace Hello100Admin.Modules.Admin.Infrastructure.Models;

/// <summary>
/// Member DB 모델 (vm_users 뷰 기반)
/// </summary>
public class MemberDbModel
{
    public required string Uid { get; set; } // 고객아이디
    public int Mid { get; set; } // 멤버아이디
    public string Name { get; set; } = string.Empty; // 멤버이름
    public string? Pwd { get; set; } // 비밀번호
    public string? SnsId { get; set; } // SNS아이디
    public string? Email { get; set; } // 이메일(암호화)
    public string? Birthday { get; set; } // 생년월일(암호화)
    public string? Sex { get; set; } // 성별
    public string? Phone { get; set; } // 전화번호(암호화)
    public string? Photo { get; set; } // 프로필사진
    public string DelYn { get; set; } = "N"; // 삭제여부
    public string LoginType { get; set; } = "E"; // 로그인타입
    public required string LoginTypeName { get; set; } // 로그인타입명
    public long? Said { get; set; } // 공통등급
    public long RegDt { get; set; } // 등록일시
    public string? RegDtView { get; set; } // 등록일시(뷰)
    public long LastLoginDt { get; set; } // 마지막 로그인 일시
    public string? LastLoginDtView { get; set; } // 마지막 로그인 일시(뷰)
    public string? LastLoginDtViewNew { get; set; } // 마지막 로그인 일시(뷰, 신규포맷???)
    public byte UserRole { get; set; } // 사용자권한(0:일반, 1:테스트계정)
}

/// <summary>
/// MemberFamily DB 모델 (tb_member 테이블)
/// </summary>
public class MemberFamilyDbModel
{
    public required string Uid { get; set; } // 고객아이디
    public int Mid { get; set; } // 멤버아이디
    public string Name { get; set; } = string.Empty; // 멤버이름
    public string Birthday { get; set; } = string.Empty; // 생년월일
    public required string Sex { get; set; } // 성별
    public long RegDt { get; set; } // 등록일시
}