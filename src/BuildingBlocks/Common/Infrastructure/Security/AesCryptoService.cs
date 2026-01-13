using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;

/// <summary>
/// AES-256 & DES 암호화 서비스 구현
/// 레거시 시스템과 동일한 암호화 방식 사용 (다중 키 지원)
/// </summary>
public class AesCryptoService : ICryptoService
{
    private readonly Dictionary<CryptoKeyType, byte[]> _aesKeys;
    private readonly byte[] _aesIv;
    private readonly byte[] _desKey;
    private readonly ILogger<AesCryptoService> _logger;

    public AesCryptoService(IConfiguration configuration, ILogger<AesCryptoService> logger)
    {
        _logger = logger;

        // AES IV는 모든 키 타입에서 공통으로 사용 (Zero IV - 레거시 호환)
        var ivString = configuration["Crypto:IV"]
            ?? throw new InvalidOperationException("Crypto:IV is not configured");

        _aesIv = Encoding.UTF8.GetBytes(ivString);
        if (_aesIv.Length != 16)
            throw new InvalidOperationException($"Invalid IV length: {_aesIv.Length} bytes. AES requires 16 bytes.");

        // 각 키 타입별 AES 키 로드
        _aesKeys = new Dictionary<CryptoKeyType, byte[]>();

        // 기본 AES 키 (필수)
        var defaultKeyString = configuration["Crypto:Key:Default"]
            ?? throw new InvalidOperationException("Crypto:Key:Default is not configured");
        _aesKeys[CryptoKeyType.Default] = ValidateAndConvertAesKey(defaultKeyString, "Default");

        // 이메일/이름 AES 키 (선택적 - 없으면 기본 키 사용)
        var emailNameKeyString = configuration["Crypto:Key:EmailName"];
        if (!string.IsNullOrEmpty(emailNameKeyString))
        {
            var key = ValidateAndConvertAesKey(emailNameKeyString, "EmailName");
            _aesKeys[CryptoKeyType.Email] = key;
            _aesKeys[CryptoKeyType.Name] = key; // Email과 Name은 동일한 키 사용
        }
        else
        {
            _aesKeys[CryptoKeyType.Email] = _aesKeys[CryptoKeyType.Default];
            _aesKeys[CryptoKeyType.Name] = _aesKeys[CryptoKeyType.Default];
        }

        var sellerKeyString = configuration["Crypto:Key:Seller"];
        if (!string.IsNullOrEmpty(sellerKeyString))
        {
            var key = ValidateAndConvertAesKey(sellerKeyString, "Seller");
            _aesKeys[CryptoKeyType.Seller] = key;
        }
        else
        {
            _aesKeys[CryptoKeyType.Seller] = _aesKeys[CryptoKeyType.Default];
        }

        // DES 파라미터 키 로드 (선택적 - 없으면 기본값)
        var paramKeyString = configuration["Crypto:Key:Parameter"];
        if (!string.IsNullOrEmpty(paramKeyString))
        {
            // DES는 8바이트 키 필요 (ASCII 직접 변환 - 레거시 호환)
            _desKey = Encoding.ASCII.GetBytes(paramKeyString);
            if (_desKey.Length != 8)
                throw new InvalidOperationException(
                    $"Invalid DES key length: {_desKey.Length} bytes. DES requires 8 bytes.");
        }
        else
        {
            // 기본값: "12345678" (레거시 호환)
            _desKey = Encoding.ASCII.GetBytes("12345678");
        }
    }

    private byte[] ValidateAndConvertAesKey(string keyString, string keyName)
    {
        // 레거시 시스템 호환: UTF-8 문자열을 직접 바이트로 변환
        // Base64가 아닌 평문 문자열 (예: "my32characterpasswordkey1234567")
        var key = Encoding.UTF8.GetBytes(keyString);
        // 레거시 시스템 호환: 키 길이 검증 없음 주석 처리
        // if (key.Length != 32)
        //     throw new InvalidOperationException(
        //         $"Invalid key length for {keyName}: {key.Length} bytes. AES-256 requires 32 bytes.");
        return key;
    }

    /// <summary>
    /// 기본 키로 암호화 (기존 코드 호환)
    /// </summary>
    public string Encrypt(string plainText)
    {
        return Encrypt(plainText, CryptoKeyType.Default);
    }

    /// <summary>
    /// 기본 키로 복호화 (기존 코드 호환)
    /// </summary>
    public string Decrypt(string encryptedText)
    {
        return Decrypt(encryptedText, CryptoKeyType.Default);
    }

    /// <summary>
    /// 지정된 키 타입으로 AES-256 암호화
    /// </summary>
    public string Encrypt(string plainText, CryptoKeyType keyType)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _aesKeys[keyType];
            aes.IV = _aesIv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }
        catch (Exception ex)
        {
            throw new CryptographicException($"Failed to encrypt data with {keyType} key: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 지정된 키 타입으로 AES-256 복호화
    /// </summary>
    public string Decrypt(string encryptedText, CryptoKeyType keyType)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return encryptedText;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _aesKeys[keyType];
            aes.IV = _aesIv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (FormatException)
        {
            // Base64 디코딩 실패 - 평문이거나 다른 형식일 수 있음
            _logger.LogWarning("Failed to decode Base64 (likely plain text): {EncryptedText}", 
                encryptedText.Substring(0, Math.Min(20, encryptedText.Length)));
            return encryptedText; // 원본 반환
        }
        catch (CryptographicException ex)
        {
            // 복호화 실패 - 잘못된 키이거나 손상된 데이터
            _logger.LogWarning("Failed to decrypt with {KeyType} key: {Message}. Data: {EncryptedText}", 
                keyType, ex.Message, encryptedText.Substring(0, Math.Min(20, encryptedText.Length)));
            return encryptedText; // 원본 반환
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during decryption with {KeyType} key", keyType);
            throw new CryptographicException($"Failed to decrypt data with {keyType} key: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// DES로 파라미터 암호화 (레거시 paramEncrypt 호환)
    /// Key와 IV에 동일한 값 사용
    /// </summary>
    public string EncryptParameter(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        try
        {
            using var des = DES.Create();
            des.Key = _desKey;
            des.IV = _desKey; // Key와 IV 동일 (레거시 호환)
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            using var encryptor = des.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }
        catch (Exception ex)
        {
            throw new CryptographicException($"Failed to encrypt parameter: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// DES로 파라미터 복호화 (레거시 paramEncrypt 호환)
    /// Key와 IV에 동일한 값 사용
    /// </summary>
    public string DecryptParameter(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return encryptedText;

        try
        {
            using var des = DES.Create();
            des.Key = _desKey;
            des.IV = _desKey; // Key와 IV 동일 (레거시 호환)
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            using var decryptor = des.CreateDecryptor();
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            throw new CryptographicException($"Failed to decrypt parameter: {ex.Message}", ex);
        }
    }
}
