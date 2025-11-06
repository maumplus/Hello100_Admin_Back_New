using System.Net;
using System.Text.Json;
using Hello100Admin.BuildingBlocks.Common.Errors;

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
            await WriteErrorResponse(context, status, ex.ErrorCode, ex.Message, ex.Details);
        }
        catch (UnauthorizedAccessException ex)
        {
            // 인증/인가 관련 예외는 401로 매핑
            _logger.LogWarning(ex, "Unauthorized access");
            await WriteErrorResponse(context, HttpStatusCode.Unauthorized, "AUTH_UNAUTHORIZED", "인증이 필요합니다.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, ErrorCodes.Unknown, "서버 내부 오류가 발생했습니다.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string code, string message, object? details = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var error = new ApiErrorResponse(code, message, details);
        var json = JsonSerializer.Serialize(error);
        await context.Response.WriteAsync(json);
    }
}
