namespace Hello100Admin.BuildingBlocks.Common.Errors;

public class ApiErrorDetail
{
    public int ErrorCode { get; set; } = (int)GlobalErrorCode.UnexpectedError;
    public string ErrorName { get; set; } = GlobalErrorCode.UnexpectedError.ToString();
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
}

public class ApiResponse
{
    public ApiErrorDetail? Error { get; set; }

    public ApiResponse()
    {
        Error = null;
    }

    public ApiResponse(int errorCode, string errorName, string message, object? details = null)
    {
        Error = new ApiErrorDetail
        {
            ErrorCode = errorCode,
            ErrorName = errorName,
            Message = message,
            Details = details
        };
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public ApiResponse(T data)
        : base()
    {
        Data = data;
    }

    public ApiResponse(T data, int errorCode, string errorName, string message, object? details = null)
    : base(errorCode, errorName, message, details)
    {
        Data = data;
    }
}