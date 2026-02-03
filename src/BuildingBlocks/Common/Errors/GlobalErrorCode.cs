using System.ComponentModel;

namespace Hello100Admin.BuildingBlocks.Common.Errors;

/// <summary>
/// 범위 : 1001 ~ 2000 && 900000 ~ 999999
/// 추후 에러코드 관련 정의 필요 [Module당 Number or 별도 Code 세팅]
/// </summary>
public enum GlobalErrorCode
{
    [Description("시스템 오류가 발생했습니다. 관리자에게 문의해주세요.")]
    UnexpectedError = 999999,
    [Description("알 수 없는 오류가 발생했습니다. 관리자에게 문의해주세요.")]
    UnknownError = 999998,
    [Description("요청 데이터 검증 간 에러가 발생했습니다.")]
    ValidationError = 1001,
    [Description("사용자 정보를 찾지 못했습니다.")]
    UserNotFound = 1002,
    [Description("계정 ID 또는 비밀번호가 올바르지 않습니다.")]
    AuthFailed = 1003,
    [Description("데이터 충돌이 발생했습니다. 다시 시도해주세요.")]
    Conflict = 1004,
    [Description("인증 또는 권한이 없습니다. 확인 후 다시 시도해주세요.")]
    UnauthorizedError = 1005,
    [Description("인증정보가 만료되었습니다. 다시 로그인해주세요.")]
    ExpiredAuthenticationInfo = 1006,
    [Description("토큰 서명이 유효하지 않습니다.")]
    AuthInvalidSignature = 1007,
    [Description("토큰 복호화에 실패했습니다.")]
    AuthDecryptionFailed = 1008,
    [Description("토큰 형식이 올바르지 않습니다.")]
    AuthMalformed = 1009,
    [Description("접근 권한이 없습니다. 관리자에게 권한 요청 후 다시 시도하세요.")]
    ForbiddenRoleRequired = 1010,
    [Description("액세스 토큰 정보가 유효하지 않습니다.")]
    InvalidAccessToken = 1011,
    [Description("데이터 조회 중 에러가 발생하였습니다. 다시 시도해주세요.")]
    DataQueryError = 1012,
    [Description("데이터 등록 중 에러가 발생하였습니다. 다시 시도해주세요.")]
    DataInsertError = 1013,
    [Description("데이터 수정 중 에러가 발생하였습니다. 다시 시도해주세요.")]
    DataUpdateError = 1014,
    [Description("데이터 삭제 중 에러가 발생하였습니다. 다시 시도해주세요.")]
    DataDeleteError = 1015,
    [Description("엑셀 조회 조건에 해당하는 데이터가 존재하지 않습니다. 조건 확인 후 다시 시도해주세요.")]
    NoDataForExcelExport = 1016,
    [Description("요청 데이터가 비어있습니다. 확인 후 다시 시도해주세요.")]
    EmptyRequestBody = 1017,
    [Description("암호화 텍스트가 비어있습니다. 확인 후 다시 시도해주세요.")]
    EmptyEncryptPlainText = 1018,
    [Description("요청에 유효하지 않거나 지원되지 않는 파라미터 값이 포함되어 있습니다. 확인 후 다시 시도해주세요.")]
    InvalidInputParameter = 1019,
    [Description("인증코드가 유효하지 않습니다. 확인 후 다시 시도해주세요.")]
    InvalidVerificationCode = 1020,
    [Description("인증코드를 요청해주세요.")]
    RequestVerificationCode = 1021,
    [Description("인증코드 요청에 실패하였습니다. 다시 시도해주세요.")]
    FailedRequestVerificationCode = 1022,
    [Description("인증코드가 만료되었습니다.")]
    ExpiredVerificationCode = 1023,
}
