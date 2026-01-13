namespace Hello100Admin.BuildingBlocks.Common.Errors;

public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public ApiErrorDetail Error { get; set; }

    public ApiErrorResponse(int errorCode, string errorName, string message, object? details = null)
    {
        Success = false;
        Error = new ApiErrorDetail
        {
            ErrorCode = errorCode,
            ErrorName = errorName,
            Message = message,
            Details = details
        };
    }
}

public class ApiErrorDetail
{
    public int ErrorCode { get; set; } = (int)GlobalErrorCode.UnexpectedError;
    public string ErrorName { get; set; } = GlobalErrorCode.UnexpectedError.ToString();
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
}
