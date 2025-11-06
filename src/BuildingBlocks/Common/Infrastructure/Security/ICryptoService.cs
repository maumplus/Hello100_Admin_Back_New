namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;

/// <summary>
/// AES 암호화 키 타입 (레거시 시스템 호환)
/// </summary>
public enum CryptoKeyType
{
    /// <summary>
    /// 기본 키 (_AES256Key)
    /// </summary>
    Default = 0,
    
    /// <summary>
    /// 이름 전용 키 (_AES256Key_Email_Name)
    /// </summary>
    Name = 91,
    
    /// <summary>
    /// 이메일 전용 키 (_AES256Key_Email_Name)
    /// </summary>
    Email = 92
}

/// <summary>
/// 암호화/복호화 서비스 인터페이스
/// 레거시 시스템의 AES-256 암호화와 호환
/// </summary>
public interface ICryptoService
{
    /// <summary>
    /// 평문을 AES-256으로 암호화 (기본 키 사용)
    /// </summary>
    /// <param name="plainText">암호화할 평문</param>
    /// <returns>Base64 인코딩된 암호화 문자열</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// AES-256으로 암호화된 문자열을 복호화 (기본 키 사용)
    /// </summary>
    /// <param name="encryptedText">Base64 인코딩된 암호화 문자열</param>
    /// <returns>복호화된 평문</returns>
    string Decrypt(string encryptedText);
    
    /// <summary>
    /// 지정된 키 타입으로 평문을 AES-256으로 암호화
    /// </summary>
    /// <param name="plainText">암호화할 평문</param>
    /// <param name="keyType">사용할 키 타입</param>
    /// <returns>Base64 인코딩된 암호화 문자열</returns>
    string Encrypt(string plainText, CryptoKeyType keyType);

    /// <summary>
    /// 지정된 키 타입으로 AES-256 암호화된 문자열을 복호화
    /// </summary>
    /// <param name="encryptedText">Base64 인코딩된 암호화 문자열</param>
    /// <param name="keyType">사용할 키 타입</param>
    /// <returns>복호화된 평문</returns>
    string Decrypt(string encryptedText, CryptoKeyType keyType);
    
    /// <summary>
    /// DES로 파라미터 암호화 (레거시 paramEncrypt 호환)
    /// Key와 IV에 동일한 값 사용
    /// </summary>
    /// <param name="plainText">암호화할 평문</param>
    /// <returns>Base64 인코딩된 암호화 문자열</returns>
    string EncryptParameter(string plainText);
    
    /// <summary>
    /// DES로 파라미터 복호화 (레거시 paramEncrypt 호환)
    /// Key와 IV에 동일한 값 사용
    /// </summary>
    /// <param name="encryptedText">Base64 인코딩된 암호화 문자열</param>
    /// <returns>복호화된 평문</returns>
    string DecryptParameter(string encryptedText);
}
