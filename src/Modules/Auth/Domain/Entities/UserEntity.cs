using Hello100Admin.BuildingBlocks.Common.Domain;

namespace Hello100Admin.Modules.Auth.Domain.Entities;

/// <summary>
/// 관리자 정보 엔티티 (tb_admin)
/// </summary>
public class UserEntity : AggregateRoot<string>
{
    /// <summary>
    /// 관리자아이디
    /// </summary>
    public required string Aid { get; set; }
    /// <summary>
    /// 계정아이디
    /// </summary>
    public required string AccId { get; set; }
    /// <summary>
    /// 계정비밀번호
    /// </summary>
    public required string AccPwd { get; set; }
    /// <summary>
    /// 요양기관번호
    /// </summary>
    public string HospNo { get; set; } = string.Empty;
    /// <summary>
    /// 요양기관키
    /// </summary>
    public string HospKey { get; set; } = string.Empty;
    /// <summary>
    /// 등급(tb_common:07)
    /// </summary>
    public required string Grade { get; set; }
    /// <summary>
    /// 이름
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// 삭제유무
    /// </summary>
    public required string DelYn { get; set; }
    /// <summary>
    /// 마지막 로그인 일시
    /// </summary>
    public int? LastLoginDt { get; set; }
    /// <summary>
    /// 마지막 로그인 일시
    /// </summary>
    public string? LastLoginDtStr { get; set; }
    /// <summary>
    /// hosp 매핑 최종 동의 시간
    /// </summary>
    public int? AgreeDt { get; set; }
    /// <summary>
    /// 권한 그룹 ID (tb_admin_role_group.role_id)
    /// </summary>
    public int? RoleId { get; set; }
    /// <summary>
    /// 투팩터 인증 사용 여부(Y/N)
    /// </summary>
    public string Use2fa { get; set; } = string.Empty;
    /// <summary>
    /// 계정 잠금 여부(Y/N)
    /// </summary>
    public string AccountLocked { get; set; } = string.Empty;
    /// <summary>
    /// 로그인 실패 횟수
    /// </summary>
    public int? LoginFailCount { get; set; }
    /// <summary>
    /// 마지막 비밀번호 변경 일시
    /// </summary>
    public int? LastPwdChangeDt { get; set; }
    /// <summary>
    /// 마지막 비밀번호 변경 일시
    /// </summary>
    public string? LastPwdChangeDtStr { get; set; }
    /// <summary>
    /// 엑세스 토큰
    /// </summary>
    public string? AccessToken { get; set; }
    /// <summary>
    /// 리프레시 토큰
    /// </summary>
    public string? RefreshToken { get; set; }

    // ============================================================================
    // 인가(Authorization) 관련 헬퍼 메서드
    // ============================================================================

    /// <summary>
    /// 로그인 성공 기록
    /// </summary>
    public void RecordLogin()
    {
        LoginFailCount = 0;  // 로그인 성공 시 실패 횟수 초기화
        AccountLocked = "N";
    }

    /// <summary>
    /// 로그인 실패 기록 (5회 실패 시 자동 잠금)
    /// 엑세스 토큰
    /// </summary>
    public void RecordLoginFailure()
    {
        LoginFailCount++;
        if (LoginFailCount >= 5)  // 5회 실패 시 계정 잠금
        {
            AccountLocked = "Y";
        }
    }

    /// <summary>
    /// 로그인 가능 여부 체크
    /// </summary>
    public bool CanLogin() => DelYn == "N" && AccountLocked == "N";
}
