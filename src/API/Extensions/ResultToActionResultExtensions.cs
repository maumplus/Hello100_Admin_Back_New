using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Extensions;

public static class ResultToActionResultExtensions
{
    /// <summary>
    /// Result<T>를 ControllerBase의 IActionResult로 변환합니다.
    /// authEndpoint 플래그를 통해 인증 관련 엔드포인트에서의 ErrorCodes.AuthFailed -> 401 매핑을 제어할 수 있습니다.
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller, bool authEndpoint = false)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (controller == null) throw new ArgumentNullException(nameof(controller));

        var errorInfo = result.ErrorInfo;

        if (result.IsSuccess == true)
        {
            if (errorInfo == null)
            {
                return controller.Ok(new ApiSuccessResponse<T>(result.Data));
            }
            else
            {
                int bizErrorCode = errorInfo.Code;
                string bizErrorName = errorInfo.Name;
                string bizErrorMessage = errorInfo.Message;

                return controller.Ok(new ApiSuccessResponse<T>(result.Data, bizErrorCode, bizErrorName, bizErrorMessage));
            }
        }

        var error = errorInfo ?? GlobalErrorCode.UnexpectedError.ToError();

        int errorCode = error.Code;
        string errorName = error.Name;
        string errorMessage = error.Message;
        object? details = result.Details;

        if (errorCode == (int)GlobalErrorCode.UserNotFound)
            return controller.NotFound(new ApiErrorResponse(errorCode, errorName, errorMessage, details));

        if (errorCode == (int)GlobalErrorCode.ValidationError)
            return controller.BadRequest(new ApiErrorResponse(errorCode, errorName, errorMessage, details));

        if (errorCode == (int)GlobalErrorCode.AuthFailed)
        {
            if (authEndpoint)
                return controller.Unauthorized(new ApiErrorResponse(errorCode, errorName, errorMessage, details));
            // 일반 엔드포인트에서는 BadRequest로 처리
            return controller.BadRequest(new ApiErrorResponse(errorCode, errorName, errorMessage, details));
        }

        // 기타 표준 매핑(예: 충돌)
        if (errorCode == (int)GlobalErrorCode.Conflict)
            return controller.Conflict(new ApiErrorResponse(errorCode, errorName, errorMessage, details));

        // 기본: 500 내부 서버 오류
        return controller.StatusCode(500, new ApiErrorResponse(errorCode, errorName, errorMessage, details));
    }

    /// <summary>
    /// Result(non-generic) 변환 (값 없는 경우)
    /// </summary>
    public static IActionResult ToActionResult(this Result result, ControllerBase controller, bool authEndpoint = false)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (controller == null) throw new ArgumentNullException(nameof(controller));

        var errorInfo = result.ErrorInfo;

        if (result.IsSuccess == true)
        {
            if (errorInfo == null)
            {
                return controller.Ok(new ApiSuccessResponse());
            }
            else
            {
                int bizErrorCode = errorInfo.Code;
                string bizErrorName = errorInfo.Name;
                string bizErrorMessage = errorInfo.Message;

                return controller.Ok(new ApiSuccessResponse(bizErrorCode, bizErrorName, bizErrorMessage));
            }
        }

        var error = errorInfo ?? GlobalErrorCode.UnexpectedError.ToError();

        int errorCode = error.Code;
        string errorName = error.Name;
        string errorMessage = error.Message;
        object? details = result.Details;

        if (errorCode == (int)GlobalErrorCode.UserNotFound)
            return controller.NotFound(new ApiErrorResponse(errorCode, errorName, errorMessage, details));

        if (errorCode == (int)GlobalErrorCode.ValidationError)
            return controller.BadRequest(new ApiErrorResponse(errorCode, errorName, errorMessage, details));

        if (errorCode == (int)GlobalErrorCode.AuthFailed)
        {
            if (authEndpoint)
                return controller.Unauthorized(new ApiErrorResponse(errorCode, errorName, errorMessage, details));
            return controller.BadRequest(new ApiErrorResponse(errorCode, errorName, errorMessage, details));
        }

        if (errorCode == (int)GlobalErrorCode.Conflict)
            return controller.Conflict(new ApiErrorResponse(errorCode, errorName, errorMessage, details));

        return controller.StatusCode(500, new ApiErrorResponse(errorCode, errorName, errorMessage, details));
    }
}
