using System.Net;
using System.Text.Json;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;

namespace Hello100Admin.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            // DomainException 서브타입이 제공하는 StatusCode를 사용하도록 변경
            var status = ex.StatusCode;
            _logger.LogWarning(ex, "Domain error: {ErrorCode} => {StatusCode}", ex.ErrorCode, (int)status);
            await WriteErrorResponse(context, status, ex.ErrorCode, ex.ErrorName, ex.Message, ex.Details);
        }
        catch (UnauthorizedAccessException ex)
        {
            // 인증/인가 관련 예외는 401로 매핑
            _logger.LogWarning(ex, "Unauthorized access");
            var errorInfo = GlobalErrorCode.UnauthorizedError.ToError();
            await WriteErrorResponse(context, HttpStatusCode.Unauthorized, errorInfo.Code, errorInfo.Name, errorInfo.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            var errorInfo = GlobalErrorCode.UnexpectedError.ToError();
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, errorInfo.Code, errorInfo.Name, errorInfo.Message);
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, int code, string name, string message, object? details = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var error = new ApiErrorResponse(code, name, message, details);
        var json = JsonSerializer.Serialize(error);
        await context.Response.WriteAsync(json);
    }
}
