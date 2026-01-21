using System.Net;
using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.BuildingBlocks.Common.Errors;

/// <summary>
/// 기본 도메인 예외입니다. 서브클래스는 적절한 HTTP 상태 코드를 오버라이드할 수 있습니다.
/// 기본값은 400 BadRequest입니다.
/// </summary>
public class DomainException : Exception
{
    public int ErrorCode { get; }
    public string ErrorName { get; }
    public object? Details { get; }

    /// <summary>
    /// 도메인 예외에 매핑될 기본 HTTP 상태 코드입니다. 필요시 서브클래스에서 재정의하세요.
    /// </summary>
    public virtual HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public DomainException(int errorCode, string errorName, string message, object? details = null)
        : base(message)
    {
        ErrorCode = errorCode;
        ErrorName = errorName;
        Details = details;
    }

    public DomainException(ErrorInfo errorInfo, object? details = null)
    : base(errorInfo.Message)
    {
        ErrorCode = errorInfo.Code;
        ErrorName = errorInfo.Name;
        Details = details;
    }
}

// 자주 사용하는 도메인 예외 서브타입들
public class BizException : DomainException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.OK;

    public BizException(int errorCode, string errorName, string message, object? details = null)
        : base(errorCode, errorName, message, details) { }

    public BizException(ErrorInfo errorInfo, object? details = null)
       : base(errorInfo, details) { }
}

public class NotFoundException : DomainException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public NotFoundException(int errorCode, string errorName, string message, object? details = null)
        : base(errorCode, errorName, message, details) { }

    public NotFoundException(ErrorInfo errorInfo, object? details = null)
        : base(errorInfo, details) { }
}

public class ConflictException : DomainException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

    public ConflictException(int errorCode, string errorName, string message, object? details = null)
        : base(errorCode, errorName, message, details) { }

    public ConflictException(ErrorInfo errorInfo, object? details = null)
        : base(errorInfo, details) { }
}

public class ValidationException : DomainException
{
    // 기본적으로 400 BadRequest를 사용
    public ValidationException(int errorCode, string errorName, string message, object? details = null)
        : base(errorCode, errorName, message, details) { }

    public ValidationException(ErrorInfo errorInfo, object? details = null)
        : base(errorInfo, details) { }
}
