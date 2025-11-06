namespace Hello100Admin.BuildingBlocks.Common.Errors;

public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public ApiErrorDetail Error { get; set; }

    public ApiErrorResponse(string code, string message, object? details = null)
    {
        Success = false;
        Error = new ApiErrorDetail
        {
            Code = code,
            Message = message,
            Details = details
        };
    }
}

public class ApiErrorDetail
{
    public string Code { get; set; } = ErrorCodes.Unknown;
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
}
