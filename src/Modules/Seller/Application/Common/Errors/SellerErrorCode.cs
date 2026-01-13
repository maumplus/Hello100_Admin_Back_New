using System.ComponentModel;

namespace Hello100Admin.Modules.Seller.Application.Common.Errors
{
    /// <summary>
    /// 범위 : 2001 ~ 4000
    /// 추후 에러코드 관련 정의 필요 [Module당 Number or 별도 Code 세팅]
    /// </summary>
    public enum SellerErrorCode
    {
        [Description("병원을 찾지 못했습니다.")]
        HospitalNotFound = 2001,
        [Description("송금 데이터 저장을 실패했습니다.")]
        HospitalSellerInsertError = 2002,
        [Description("KCP 정보 동기화 요청 간 오류가 발생했습니다.")]
        KcpSellerSyncError = 2003,
        [Description("KCP 정보 동기화에 실패했습니다.")]
        KcpSellerSyncUpdateError = 2004,
        [Description("계좌 정보를 찾지 못했습니다.")]
        NotFoundSeller = 2005,
        [Description("송금 요청 등록에 실패했습니다.")]
        SellerRemitInsertError = 2006,
        [Description("요청 가능한 상태가 아닙니다.")]
        InvalidStateForRequest = 2007,
        [Description("KCP 응답이 없습니다.")]
        KcpNoResponse = 2008,
        [Description("KCP 잔액 조회 간 오류가 발생했습니다.")]
        KcpBalanceInquiryFailed = 2009,
        [Description("판매자 송금 목록 조회에 실패했습니다.")]
        SellerRemitListInquiryFailed = 2010,
        [Description("판매자 송금 정보 삭제를 실패했습니다.")]
        SellerRemitDeleteFailed = 2011,
        [Description("이미 송금이 완료되어 삭제가 불가능 합니다.")]
        SellerRemitAlreadyCompleted = 2012,
        [Description("이미 삭제가 완료된 건입니다.")]
        SellerRemitAlreadyDeleted = 2013,
        [Description("판매자 송금 정보 삭제 간 에러가 발생하였습니다. 관리자에게 문의하거나 다시 시도해주세요.")]
        SellerRemitDeleteFailedError = 2014,
        [Description("병원 판매자 계좌 활성화/비활성화 처리 간 에러가 발생하였습니다. 관리자에게 문의하거나 다시 시도해주세요.")]
        UpdateSellerRemitEnabledFailedError = 2015,
        [Description("은행 이미지 정보가 존재하지 않습니다.")]
        NotFoundBankList = 2016
    }
}
