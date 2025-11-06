using System.Text;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;

namespace Hello100Admin.Integration.Shared;

/// <summary>
/// 테스트용 간단한 CryptoService 구현
/// 실제 AES 암호화 대신 Base64 인코딩 사용 (예측 가능한 테스트를 위해)
/// </summary>
/// <remarks>
/// Infrastructure Unit Tests와 동일한 구현을 사용하여 일관성 유지
/// 프로덕션에서는 절대 사용하지 말것!
/// </remarks>
public class TestCryptoService : ICryptoService
{
    /// <summary>
    /// 평문을 Base64로 인코딩 (암호화 시뮬레이션)
    /// </summary>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        var bytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Base64를 평문으로 디코딩 (복호화 시뮬레이션)
    /// </summary>
    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return encryptedText;

        try
        {
            var bytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException)
        {
            // Base64 포맷이 아닌 경우 원본 반환 (테스트 안정성)
            return encryptedText;
        }
    }

    /// <summary>
    /// 키 타입 지정 암호화 (기본 Encrypt 호출)
    /// </summary>
    public string Encrypt(string plainText, CryptoKeyType keyType)
    {
        return Encrypt(plainText);
    }

    /// <summary>
    /// 키 타입 지정 복호화 (기본 Decrypt 호출)
    /// </summary>
    public string Decrypt(string encryptedText, CryptoKeyType keyType)
    {
        return Decrypt(encryptedText);
    }

    /// <summary>
    /// DES 파라미터 암호화 (기본 Encrypt 호출)
    /// </summary>
    public string EncryptParameter(string plainText)
    {
        return Encrypt(plainText);
    }

    /// <summary>
    /// DES 파라미터 복호화 (기본 Decrypt 호출)
    /// </summary>
    public string DecryptParameter(string encryptedText)
    {
        return Decrypt(encryptedText);
    }
}
