namespace Hello100Admin.BuildingBlocks.Common.Application;

public static class ResultExtensions
{
    // 순수 Result 변환/헬퍼 메서드만 이곳에 작성 (API 응답 변환은 Controller/API 계층에서 처리)
    public static Result WithError(this Result result, ErrorInfo errorInfo, object? details = null)
    {
        result.ErrorInfo = errorInfo;
        result.Details = details;

        return result;
    }

    public static Result<T> WithError<T>(this Result<T> result, ErrorInfo errorInfo, object? details = null)
    {
        result.ErrorInfo = errorInfo;
        result.Details = details;

        return result;
    }
}
