using Dapper;
using Hello100Admin.BuildingBlocks.Common.Domain;
using System.Data;

namespace Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;

/// <summary>
/// Dapper에서 int(Unix timestamp) <-> DateTime 변환을 자동화하는 타입 핸들러
/// </summary>
public class UnixTimestampDateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = (int)(value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    public override DateTime Parse(object value)
    {
        if (value is int unix)
            return new DateTime(1970, 1, 1).AddSeconds(unix).ToUniversalTime();
        if (value is long unixLong)
            return new DateTime(1970, 1, 1).AddSeconds(unixLong).ToUniversalTime();
        throw new DataException("Cannot convert value to DateTime");
    }
}

/// <summary>
/// Dapper에서 EncryptedData <-> string 변환을 자동화하는 타입 핸들러
/// </summary>
public class EncryptedDataTypeHandler : SqlMapper.TypeHandler<EncryptedData>
{
    public override void SetValue(IDbDataParameter parameter, EncryptedData? value)
    {
        // DB에는 암호화된 문자열만 저장
        parameter.Value = value?.EncryptedValue ?? string.Empty;
    }

    public override EncryptedData Parse(object value)
    {
        // DB에서 읽은 암호화 문자열을 EncryptedData 객체로 변환
        if (value is string s)
            return EncryptedData.FromEncrypted(s);
        throw new DataException($"Cannot convert {value?.GetType()} to EncryptedData");
    }
}
