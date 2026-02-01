using Hello100Admin.BuildingBlocks.Common.Domain;

namespace Hello100Admin.Modules.Admin.Domain.Entities;

/// <summary>
/// AdminUser 정보 엔티티 (tb_admin)
/// </summary>
public class AdminUserEntity : AggregateRoot<string>
{
    /// <summary>
    /// 관리자아이디 (PK)
    /// </summary>
    public required string Aid { get; set; } // varchar(8)

    /// <summary>
    /// 계정아이디
    /// </summary>
    public required string AccId { get; set; } // varchar(50)

    /// <summary>
    /// 계정비밀번호
    /// </summary>
    public required string AccPwd { get; set; } // varchar(128)

    /// <summary>
    /// 요양기관키
    /// </summary>
    public string? HospKey { get; set; }
    /// <summary>
    /// 요양기관번호
    /// </summary>
    public string? HospNo { get; set; } // varchar(50)

    /// <summary>
    /// 등급(tb_common:07)
    /// </summary>
    public required string Grade { get; set; } // char(1)

    /// <summary>
    /// 관리자 이름
    /// </summary>
    public required string Name { get; set; } // varchar(128)

    /// <summary>
    /// 전화번호
    /// </summary>
    public string? Tel { get; set; } // varchar(20)

    /// <summary>
    /// 삭제유무 ('N': 미삭제, 'Y': 삭제)
    /// </summary>
    public required string DelYn { get; set; } // char(1) 
    /// <summary>
    /// 등록날짜
    /// </summary>
    public DateTime RegDt { get; set; }
    /// <summary>
    /// 마지막 로그인 일시
    /// </summary>
    public DateTime? LastLoginAt { get; set; } 
    /// <summary>
    /// 계정 잠금 여부 (0: 미사용, 1: 사용)
    /// </summary>
    public required string AccountLocked { get; set; }

    /// <summary>
    /// 로그인 실패 횟수
    /// </summary>
    public int LoginFailCount { get; set; }

    /// <summary>
    /// 리프레시 토큰
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 엑세스 토큰
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 승인여부 (0: 미승인, 1: 승인)
    /// </summary>
    public required string Approved { get; set; } // char(1)

    /// <summary>
    /// 활성여부 (0: 비활성, 1:활성)
    /// </summary>
    public required string Enabled { get; set; } // char(1) 
}
