using Hello100Admin.BuildingBlocks.Common.Domain;

namespace Hello100Admin.Modules.Auth.Domain.Entities;

/// <summary>
/// 관리자 정보 엔티티 (tb_admin)
/// </summary>
public class User : AggregateRoot<string>
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
    /// 요양기관번호
    /// </summary>
    public string? HospNo { get; set; } // varchar(50)

    /// <summary>
    /// 등급(tb_common:07)
    /// </summary>
    public required string Grade { get; set; } // char(1)

    /// <summary>
    /// 이름
    /// </summary>
    public required string Name { get; set; } // varchar(128)

    /// <summary>
    /// 삭제유무 ('N': 미삭제, 'Y': 삭제)
    /// </summary>
    public required string DelYn { get; set; } // char(1) 

    /// <summary>
    /// 마지막 로그인 일시
    /// </summary>
    public DateTime? LastLoginDt { get; set; } 
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

    // ============================================================================
    // 인가(Authorization) 관련 헬퍼 메서드
    // ============================================================================
    
    /// <summary>
    /// 로그인 성공 기록
    /// </summary>
    public void RecordLogin()
    {
        LastLoginDt = DateTime.UtcNow;
        LoginFailCount = 0;  // 로그인 성공 시 실패 횟수 초기화
        AccountLocked = "1";
    }

    /// <summary>
    /// 로그인 실패 기록 (5회 실패 시 자동 잠금)
    /// </summary>
    public void RecordLoginFailure()
    {
        LoginFailCount++;
        if (LoginFailCount >= 5)  // 5회 실패 시 계정 잠금
        {
            AccountLocked = "1";
        }
    }

    /// <summary>
    /// 로그인 가능 여부 체크
    /// </summary>
    public bool CanLogin() => DelYn == "N" && Approved == "0"  && Enabled == "1" && AccountLocked == "0";
}
