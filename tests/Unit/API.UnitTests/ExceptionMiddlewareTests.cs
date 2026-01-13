using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Hello100Admin.API.Middleware;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;

namespace Hello100Admin.Tests.API
{
    public class ExceptionMiddlewareTests
    {
        private static DefaultHttpContext CreateContext()
        {
            var ctx = new DefaultHttpContext();
            ctx.Response.Body = new System.IO.MemoryStream();
            return ctx;
        }

        private static async System.Threading.Tasks.Task<string> InvokeWithExceptionAsync(System.Exception ex)
        {
            var ctx = CreateContext();
            RequestDelegate next = _ => throw ex;
            var middleware = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
            await middleware.InvokeAsync(ctx);
            ctx.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
            var json = await new System.IO.StreamReader(ctx.Response.Body).ReadToEndAsync();
            return json;
        }

        [Fact]
        public async System.Threading.Tasks.Task ValidationException_Returns_400()
        {
            var errorInfo = GlobalErrorCode.ValidationError.ToError();
            var ex = new ValidationException(errorInfo.Code, errorInfo.Name, errorInfo.Message, new[] { "a", "b" });
            var json = await InvokeWithExceptionAsync(ex);
            using var doc = JsonDocument.Parse(json);
            var code = doc.RootElement.GetProperty("Error").GetProperty("ErrorName").GetString();
            Assert.Equal(errorInfo.Name, code);
        }

        [Fact]
        public async System.Threading.Tasks.Task NotFoundException_Returns_404()
        {
            var errorInfo = GlobalErrorCode.UserNotFound.ToError();
            var ex = new NotFoundException(errorInfo.Code, errorInfo.Name, errorInfo.Message, "not found");
            var json = await InvokeWithExceptionAsync(ex);
            using var doc = JsonDocument.Parse(json);
            var code = doc.RootElement.GetProperty("Error").GetProperty("ErrorName").GetString();
            Assert.Equal(errorInfo.Name, code);
        }

        [Fact]
        public async System.Threading.Tasks.Task UnauthorizedException_Returns_401()
        {
            var ex = new UnauthorizedAccessException("no auth");
            var ctx = CreateContext();
            RequestDelegate next = _ => throw ex;
            var middleware = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
            await middleware.InvokeAsync(ctx);
            Assert.Equal((int)HttpStatusCode.Unauthorized, ctx.Response.StatusCode);
        }

        [Fact]
        public async System.Threading.Tasks.Task UnhandledException_Returns_500()
        {
            var ex = new System.Exception("boom");
            var ctx = CreateContext();
            RequestDelegate next = _ => throw ex;
            var middleware = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
            await middleware.InvokeAsync(ctx);
            Assert.Equal((int)HttpStatusCode.InternalServerError, ctx.Response.StatusCode);
        }
    }
}
