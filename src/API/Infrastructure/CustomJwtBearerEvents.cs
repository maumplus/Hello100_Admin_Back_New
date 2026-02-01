using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Hello100Admin.API.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        #region FIELD AREA ******************************************
        //private readonly JwtHelper _jwtHelper;
        #endregion

        #region CONSTRUCTOR AREA ************************************************
        /// <summary>
        /// 
        /// </summary>
        public CustomJwtBearerEvents()
        {
            //_jwtHelper = jwtHelper;
        }
        #endregion

        #region JWTBEAREREVENTS OVERRIDES AREA ***************************************
        /// <summary>
        /// 토큰 추출/스킵
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task MessageReceived(MessageReceivedContext context)
        {
            var path = context.HttpContext.Request.Path;

            var bearerToken = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(bearerToken) == false
             && bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = bearerToken.Substring("Bearer ".Length).Trim();
                context.HttpContext.Items["AccessToken"] = context.Token;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 토큰 검증 성공 후 추가 검증
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task TokenValidated(TokenValidatedContext context)
        {
            // AId, HospNo 검증
            var adminId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var hospNo = context.Principal?.FindFirst("hospital_number")?.Value;

            if (string.IsNullOrWhiteSpace(adminId) == true
             || string.IsNullOrWhiteSpace(hospNo) == true)
            {
                context.Fail("Admin Id or Hospital No is null or empty");
                this.SetCutomAuthErrorContext(context.HttpContext, GlobalErrorCode.InvalidAccessToken.ToError());
                return;
            }

            var authStore = context.HttpContext.RequestServices.GetRequiredService<IAuthStore>();

            var adminInfo = await authStore.GetAdminByAidAsync(adminId);

            if (adminInfo == null || adminInfo.HospNo != hospNo)
            {
                context.Fail("Not found admin info");
                this.SetCutomAuthErrorContext(context.HttpContext, GlobalErrorCode.InvalidAccessToken.ToError());
                return;
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 검증 실패 (토큰 검증 실패로 인한 Exception 발생)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            context.NoResult();

            var errorInfo = this.MapException(context.Exception);

            this.SetCutomAuthErrorContext(context.HttpContext, errorInfo);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 검증 실패 (미인증 사용자에게 401 Unauthorized 응답을 반환)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Challenge(JwtBearerChallengeContext context)
        {
            context.HandleResponse();

            var defaultErrorInfo = GlobalErrorCode.UnauthorizedError.ToError();

            ErrorInfo? inheritedError = null;

            if (context.HttpContext.Items["AuthErrorCode"] is int preCode
             && context.HttpContext.Items["AuthErrorName"] is string preName
             && context.HttpContext.Items["AuthMessage"] is string preMessage)
            {
                var code = preCode;
                var name = preName;
                var message = preMessage;

                inheritedError = new ErrorInfo(preCode, preName, preMessage);
            }

            await this.ResponseError(context.HttpContext, inheritedError ?? defaultErrorInfo, HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// EndPoint 접근 권한 검증 실패
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Forbidden(ForbiddenContext context)
        {
            var forbiddenError = GlobalErrorCode.ForbiddenRoleRequired.ToError();

            await this.ResponseError(context.HttpContext, forbiddenError, HttpStatusCode.Forbidden);
        }
        #endregion

        #region INTERNAL METHOD AREA *******************************************
        private async Task ResponseError(HttpContext context, ErrorInfo errorCode, HttpStatusCode status)
        {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = new ApiResponse(errorCode.Code, errorCode.Name, errorCode.Message);

            await context.Response.WriteAsync(response.ToJson());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private ErrorInfo MapException(Exception ex)
        {
            return ex switch
            {
                SecurityTokenExpiredException =>
                    GlobalErrorCode.ExpiredAuthenticationInfo.ToError(),

                SecurityTokenInvalidSignatureException =>
                    GlobalErrorCode.AuthInvalidSignature.ToError(),

                //SecurityTokenInvalidAudienceException =>
                //    ("ErrorCode.AuthInvalidAudience", "토큰의 Audience가 올바르지 않습니다."),

                //SecurityTokenInvalidIssuerException =>
                //    ("ErrorCode.AuthInvalidIssuer", "토큰의 Issuer가 올바르지 않습니다."),

                //SecurityTokenNoExpirationException =>
                //    ("ErrorCode.AuthNoExp", "만료(exp) 정보가 없는 토큰입니다."),

                //SecurityTokenInvalidLifetimeException =>
                //    ("ErrorCode.AuthInvalidLifetime", "토큰의 유효기간이 올바르지 않습니다."),

                SecurityTokenDecryptionFailedException =>
                    GlobalErrorCode.AuthDecryptionFailed.ToError(),

                //SecurityTokenInvalidAlgorithmException =>
                //    ("ErrorCode.AuthInvalidAlgorithm", "허용되지 않은 알고리즘의 토큰입니다."),

                ArgumentException =>
                    GlobalErrorCode.AuthMalformed.ToError(),

                _ =>
                    GlobalErrorCode.UnauthorizedError.ToError()
            };
        }

        private void SetCutomAuthErrorContext(HttpContext context, ErrorInfo errorInfo)
        {
            context.Items["AuthErrorCode"] = errorInfo.Code;
            context.Items["AuthErrorName"] = errorInfo.Name;
            context.Items["AuthMessage"] = errorInfo.Message;
        }
        #endregion
    }
}
