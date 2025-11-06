using System.Net;

namespace Hello100Admin.BuildingBlocks.Common.Errors;

/// <summary>
/// 기본 도메인 예외입니다. 서브클래스는 적절한 HTTP 상태 코드를 오버라이드할 수 있습니다.
/// 기본값은 400 BadRequest입니다.
/// </summary>
public class DomainException : Exception
{
    public string ErrorCode { get; }
    public object? Details { get; }

    /// <summary>
    /// 도메인 예외에 매핑될 기본 HTTP 상태 코드입니다. 필요시 서브클래스에서 재정의하세요.
    /// </summary>
    public virtual HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public DomainException(string errorCode, string message, object? details = null)
        : base(message)
    {
        ErrorCode = errorCode;
        Details = details;
    }
}

// 자주 사용하는 도메인 예외 서브타입들
public class NotFoundException : DomainException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    public NotFoundException(string errorCode, string message, object? details = null)
        : base(errorCode, message, details) { }
}

public class ConflictException : DomainException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    public ConflictException(string errorCode, string message, object? details = null)
        : base(errorCode, message, details) { }
}

public class ValidationException : DomainException
{
    // 기본적으로 400 BadRequest를 사용
    public ValidationException(string errorCode, string message, object? details = null)
        : base(errorCode, message, details) { }
}
