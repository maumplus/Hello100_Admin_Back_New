namespace Hello100Admin.BuildingBlocks.Common.Errors;

public class ApiSuccessResponse
{
    public bool Success { get; set; } = true;
    public ApiErrorDetail? Error { get; set; }

    public ApiSuccessResponse()
    {
        Success = true;
    }

    public ApiSuccessResponse(int errorCode, string errorName, string message, object? details = null)
    {
        Success = true;
        Error = new ApiErrorDetail
        {
            ErrorCode = errorCode,
            ErrorName = errorName,
            Message = message,
            Details = details
        };
    }
}

public class ApiSuccessResponse<T> : ApiSuccessResponse
{
    public T? Data { get; set; }

    public ApiSuccessResponse(T data)
        : base()
    {
        Data = data;
    }

    public ApiSuccessResponse(T data, int errorCode, string errorName, string message, object? details = null)
    : base(errorCode, errorName, message, details)
    {
        Data = data;
    }
}
