using Hello100Admin.Modules.Auth.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Hello100Admin.Modules.Auth.Application.Services;

/// <summary>
/// 비밀번호 해싱 서비스 (SHA256 + Salt(aid) 사용 - 기존 레거시 시스템 호환)
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private readonly ILogger<PasswordHasher> _logger;

    public PasswordHasher(ILogger<PasswordHasher> logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// SHA256 + Salt 방식으로 비밀번호 해싱 (기존 프로덕트 방식)
    /// 주의: 이 메서드는 salt 없이 호출되지만, 실제로는 HashPasswordWithSalt를 사용해야 합니다.
    /// </summary>
    /// <param name="password">평문 비밀번호</param>
    /// <returns>해시된 비밀번호</returns>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty", nameof(password));
        }

        // 기본 구현: Salt 없이는 사용 불가
        // 실제로는 HashPasswordWithSalt(password, aid) 사용 권장
        throw new InvalidOperationException("Use HashPasswordWithSalt(password, aid) instead. Salt(aid) is required for legacy compatibility.");
    }

    /// <summary>
    /// SHA256 + Salt(aid) 방식으로 비밀번호 해싱
    /// </summary>
    /// <param name="password">평문 비밀번호</param>
    /// <param name="salt">Salt (aid 값)</param>
    /// <returns>해시된 비밀번호</returns>
    public string HashPasswordWithSalt(string password, string salt)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty", nameof(password));
        }

        if (string.IsNullOrWhiteSpace(salt))
        {
            throw new ArgumentException("Salt cannot be empty", nameof(salt));
        }

        _logger.LogDebug("Hashing password with salt for AccountId={AccountId}", salt);
        var hashedPassword = SHA256SaltHash(password, salt);
        _logger.LogDebug("Password hashed successfully for AccountId={AccountId}", salt);
        return hashedPassword;
    }

    /// <summary>
    /// 비밀번호 검증 (레거시 방식: SHA256SaltHash(password, aid))
    /// </summary>
    /// <param name="hashedPassword">저장된 해시 비밀번호</param>
    /// <param name="plainPassword">입력된 평문 비밀번호</param>
    /// <param name="salt">Salt (aid 값)</param>
    /// <returns>일치 여부</returns>
    public bool VerifyPassword(string hashedPassword, string plainPassword, string salt)
    {
        if (string.IsNullOrWhiteSpace(plainPassword) || 
            string.IsNullOrWhiteSpace(hashedPassword) || 
            string.IsNullOrWhiteSpace(salt))
        {
            _logger.LogWarning("Password verification failed: empty input for AccountId={AccountId}", salt);
            return false;
        }

        try
        {
            _logger.LogDebug("Verifying password for AccountId={AccountId}", salt);
            
            // 입력된 비밀번호 + salt로 해시 생성
            var computedHash = SHA256SaltHash(plainPassword, salt);

            // 타이밍 공격 방지를 위한 고정 시간 비교
            var isValid = CryptographicOperations.FixedTimeEquals(
                Encoding.ASCII.GetBytes(hashedPassword),
                Encoding.ASCII.GetBytes(computedHash)
            );

            if (isValid)
            {
                _logger.LogDebug("Password verification successful for AccountId={AccountId}", salt);
            }
            else
            {
                _logger.LogWarning("Password verification failed: mismatch for AccountId={AccountId}", salt);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password verification error for AccountId={AccountId}", salt);
            return false;
        }
    }

    /// <summary>
    /// 기존 인터페이스 호환성을 위한 메서드 (사용 비권장)
    /// </summary>
    public bool VerifyPassword(string hashedPassword, string plainPassword)
    {
        throw new InvalidOperationException("Use VerifyPassword(hashedPassword, plainPassword, salt) instead. Salt(aid) is required for legacy compatibility.");
    }

    /// <summary>
    /// SHA256 + Salt 해싱 (기존 프로덕트 SHA256SaltHash 메서드 정확히 재현)
    /// </summary>
    /// <param name="plainText">평문</param>
    /// <param name="saltText">Salt 문자열</param>
    /// <returns>해시 문자열 (소문자)</returns>
    private string SHA256SaltHash(string plainText, string saltText)
    {
        using var sha256 = SHA256.Create();
        
        // ASCII 인코딩으로 바이트 변환
        byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
        byte[] saltBytes = Encoding.ASCII.GetBytes(saltText);
        
        // plainBytes + saltBytes 결합
        byte[] plainWithSaltBytes = plainBytes.Concat(saltBytes).ToArray();
        
        // SHA256 해시 계산
        byte[] hashBytes = sha256.ComputeHash(plainWithSaltBytes);
        
        // BitConverter를 사용한 Hex 문자열 변환 (기존 방식)
        var sb = new StringBuilder(hashBytes.Length * 2);
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(BitConverter.ToString(hashBytes, i, 1));
        }
        
        // 소문자 변환 및 공백 제거
        string returnKey = sb.ToString().TrimEnd().ToLower();
        
        return returnKey;
    }
}
