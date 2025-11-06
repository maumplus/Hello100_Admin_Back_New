using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;

namespace Hello100Admin.BuildingBlocks.Common.Domain;

/// <summary>
/// 암호화된 데이터를 표현하는 Value Object
/// 개인정보(이름, 전화번호, 생년월일 등)를 안전하게 다루기 위한 타입
/// </summary>
public class EncryptedData : ValueObject
{
    private static ICryptoService? _cryptoService;

    /// <summary>
    /// 애플리케이션 시작 시 CryptoService를 주입
    /// Program.cs에서 한 번만 호출됨
    /// </summary>
    public static void Configure(ICryptoService cryptoService)
    {
        _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
    }

    private readonly string _encryptedValue;
    private readonly CryptoKeyType _keyType;

    /// <summary>
    /// 암호화된 값 (DB에 저장되는 값)
    /// </summary>
    public string EncryptedValue => _encryptedValue;

    /// <summary>
    /// 복호화된 평문 값 (사용 시 자동 복호화)
    /// </summary>
    public string DecryptedValue
    {
        get
        {
            if (_cryptoService == null)
                throw new InvalidOperationException(
                    "EncryptedData.Configure() must be called before using EncryptedData. " +
                    "Call it in Program.cs after building the service provider.");

            return _cryptoService.Decrypt(_encryptedValue, _keyType);
        }
    }

    private EncryptedData(string encryptedValue, CryptoKeyType keyType = CryptoKeyType.Default)
    {
        _encryptedValue = encryptedValue ?? string.Empty;
        _keyType = keyType;
    }

    /// <summary>
    /// 암호화된 값으로 EncryptedData 생성 (DB에서 읽을 때 사용)
    /// </summary>
    public static EncryptedData FromEncrypted(string encryptedValue, CryptoKeyType keyType = CryptoKeyType.Default)
    {
        return new EncryptedData(encryptedValue, keyType);
    }

    /// <summary>
    /// 평문으로 EncryptedData 생성 (저장 시 자동 암호화)
    /// </summary>
    public static EncryptedData FromPlain(string plainValue, CryptoKeyType keyType = CryptoKeyType.Default)
    {
        if (string.IsNullOrEmpty(plainValue))
            return new EncryptedData(plainValue, keyType);

        if (_cryptoService == null)
            throw new InvalidOperationException(
                "EncryptedData.Configure() must be called before using EncryptedData. " +
                "Call it in Program.cs after building the service provider.");

        var encrypted = _cryptoService.Encrypt(plainValue, keyType);
        return new EncryptedData(encrypted, keyType);
    }

    /// <summary>
    /// null 또는 빈 문자열을 안전하게 처리
    /// </summary>
    public static EncryptedData? FromPlainOrNull(string? plainValue, CryptoKeyType keyType = CryptoKeyType.Default)
    {
        return string.IsNullOrEmpty(plainValue) ? null : FromPlain(plainValue, keyType);
    }

    /// <summary>
    /// 두 EncryptedData가 같은지 비교 (암호화된 값 기준)
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return _encryptedValue;
    }

    /// <summary>
    /// 문자열 변환 시 복호화된 값 반환
    /// 주의: 로깅 시 민감정보가 노출될 수 있음
    /// </summary>
    public override string ToString() => DecryptedValue;

    /// <summary>
    /// 암호화된 상태로 문자열 반환 (로깅용)
    /// </summary>
    public string ToMaskedString() => $"***{_encryptedValue.Substring(Math.Max(0, _encryptedValue.Length - 4))}";
}
