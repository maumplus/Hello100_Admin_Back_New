namespace Hello100Admin.Modules.Auth.Application.Interfaces;

/// <summary>
/// 비밀번호 해시 서비스 인터페이스
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// 비밀번호 해시 생성 (기본 - Salt 없이는 사용 불가)
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// 비밀번호 해시 생성 with Salt (aid)
    /// </summary>
    string HashPasswordWithSalt(string password, string salt);

    /// <summary>
    /// 비밀번호 검증 (기본 - 레거시 호환성용, 사용 비권장)
    /// </summary>
    bool VerifyPassword(string hashedPassword, string providedPassword);

    /// <summary>
    /// 비밀번호 검증 with Salt (aid) - 권장
    /// </summary>
    bool VerifyPassword(string hashedPassword, string providedPassword, string salt);
}
