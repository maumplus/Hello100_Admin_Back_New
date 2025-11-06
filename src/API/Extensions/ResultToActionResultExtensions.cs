using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
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

        if (result.IsSuccess)
            return controller.Ok(new ApiSuccessResponse<T>(result.Value));

        var code = result.ErrorCode ?? ErrorCodes.Unknown;
        var message = result.Error ?? string.Empty;
        var details = result.Details;

        if (code == ErrorCodes.UserNotFound)
            return controller.NotFound(new ApiErrorResponse(code, message, details));

        if (code == ErrorCodes.Validation)
            return controller.BadRequest(new ApiErrorResponse(code, message, details));

        if (code == ErrorCodes.AuthFailed)
        {
            if (authEndpoint)
                return controller.Unauthorized(new ApiErrorResponse(code, message, details));
            // 일반 엔드포인트에서는 BadRequest로 처리
            return controller.BadRequest(new ApiErrorResponse(code, message, details));
        }

        // 기타 표준 매핑(예: 충돌)
        if (code == ErrorCodes.Conflict)
            return controller.Conflict(new ApiErrorResponse(code, message, details));

        // 기본: 500 내부 서버 오류
        return controller.StatusCode(500, new ApiErrorResponse(code, message, details));
    }

    /// <summary>
    /// Result(non-generic) 변환 (값 없는 경우)
    /// </summary>
    public static IActionResult ToActionResult(this Result result, ControllerBase controller, bool authEndpoint = false)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (controller == null) throw new ArgumentNullException(nameof(controller));

        if (result.IsSuccess)
            return controller.Ok();

        var code = result.ErrorCode ?? ErrorCodes.Unknown;
        var message = result.Error ?? string.Empty;
        var details = result.Details;

        if (code == ErrorCodes.UserNotFound)
            return controller.NotFound(new ApiErrorResponse(code, message, details));

        if (code == ErrorCodes.Validation)
            return controller.BadRequest(new ApiErrorResponse(code, message, details));

        if (code == ErrorCodes.AuthFailed)
        {
            if (authEndpoint)
                return controller.Unauthorized(new ApiErrorResponse(code, message, details));
            return controller.BadRequest(new ApiErrorResponse(code, message, details));
        }

        if (code == ErrorCodes.Conflict)
            return controller.Conflict(new ApiErrorResponse(code, message, details));

        return controller.StatusCode(500, new ApiErrorResponse(code, message, details));
    }
}
