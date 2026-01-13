namespace Hello100Admin.BuildingBlocks.Common.Application;

// 임시 경로 추후 경로 변경 예정
public sealed record ErrorInfo(int Code, string Name, string Message);

/// <summary>
/// 작업 결과를 나타내는 클래스
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public ErrorInfo? ErrorInfo { get; }
    public object? Details { get; }

    protected Result(bool isSuccess, ErrorInfo? errorInfo = null, object? details = null)
    {
        IsSuccess = isSuccess;
        ErrorInfo = errorInfo;
        Details = details;
    }

    public static Result Success()
        => new(true);

    public static Result SuccessWithError(ErrorInfo errorInfo, object? details = null)
        => new(true, errorInfo);

    public static Result Fail(ErrorInfo errorInfo, object? details = null)
        => new(false, errorInfo);

    public static Result<T> Success<T>(T data)
        => new Result<T>(data, true);

    public static Result<T> SuccessWithError<T>(ErrorInfo errorInfo, object? details = null)
        => new Result<T>(default!, true, errorInfo, details);

    public static Result<T> SuccessWithError<T>(T data, ErrorInfo errorInfo, object? details = null)
        => new Result<T>(data, true, errorInfo, details);

    public static Result<T> Fail<T>(ErrorInfo errorInfo, object? details = null)
        => new Result<T>(default!, false, errorInfo, details);
}

/// <summary>
/// 값을 포함하는 작업 결과 클래스
/// </summary>
public class Result<T> : Result
{
    public T Data { get; }

    protected internal Result(T data, bool isSuccess, ErrorInfo? errorInfo = null, object? details = null)
        : base(isSuccess, errorInfo, details)
    {
        this.Data = data;
    }
}
