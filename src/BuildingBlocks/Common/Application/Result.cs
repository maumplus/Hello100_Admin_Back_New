namespace Hello100Admin.BuildingBlocks.Common.Application;

/// <summary>
/// 작업 결과를 나타내는 클래스
/// </summary>

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }
    public object? Details { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string? error, string? errorCode = null, object? details = null)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException("성공한 결과는 에러를 가질 수 없습니다.");
        if (!isSuccess && error == null)
            throw new InvalidOperationException("실패한 결과는 에러를 가져야 합니다.");
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
        Details = details;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error, string? errorCode = null, object? details = null)
        => new(false, error, errorCode, details);
    public static Result<T> Success<T>(T value) => new Result<T>(value, true, null, null, null);
    public static Result<T> Failure<T>(string error, string? errorCode = null, object? details = null)
    => new Result<T>(default!, false, error, errorCode, details);
}

/// <summary>
/// 값을 포함하는 작업 결과 클래스
/// </summary>
public class Result<T> : Result
{
    public T Value { get; }

    protected internal Result(T value, bool isSuccess, string? error, string? errorCode, object? details)
        : base(isSuccess, error, errorCode, details)
    {
        Value = value;
    }
}
