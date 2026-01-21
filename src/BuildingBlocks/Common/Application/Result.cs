
namespace Hello100Admin.BuildingBlocks.Common.Application;

// 임시 경로 추후 경로 변경 예정
public sealed record ErrorInfo(int Code, string Name, string Message);

/// <summary>
/// 작업 결과를 나타내는 클래스
/// </summary>
public class Result
{
    public ErrorInfo? ErrorInfo { get; set; }
    public object? Details { get; set; }

    protected Result(ErrorInfo? errorInfo = null, object? details = null)
    {
        ErrorInfo = errorInfo;
        Details = details;
    }

    public static Result Success()
        => new();

    public static Result<T> Success<T>(T data = default!)
        => new Result<T>(data);
}

/// <summary>
/// 값을 포함하는 작업 결과 클래스
/// </summary>
public class Result<T> : Result
{
    public T Data { get; }

    protected internal Result(T data)
        : base()
    {
        this.Data = data;
    }
}
