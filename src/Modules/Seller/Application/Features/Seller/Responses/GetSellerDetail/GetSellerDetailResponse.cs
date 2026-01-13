namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerDetail
{
    /// <summary>
    /// 판매자 상세 정보 응답 DTO
    /// </summary>
    /// <param name="Info"></param>
    /// <param name="RemitCount"></param>
    public sealed record GetSellerDetailResponse(SellerInfo Info, SellerRemitCountInfo RemitCount);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Id">일련번호</param>
    /// <param name="HospName">병원명</param>
    /// <param name="BusinessNo">사업자 번호</param>
    /// <param name="BusinessLevel">사업자 구분</param>
    /// <param name="HospNo">요양기관 번호</param>
    /// <param name="ChartType">차트 구분</param>
    /// <param name="BankCd">은행 코드</param>
    /// <param name="BankName">은행명</param>
    /// <param name="BankImgPath">은행 로고 이미지 경로</param>
    /// <param name="DepositNo">계좌번호</param>
    /// <param name="Depositor">예금주명</param>
    /// <param name="Enabled">활성 여부</param>
    /// <param name="Etc">비고</param>
    /// <param name="IsSync">KCP 연동 여부</param>
    /// <param name="RegDt">등록일자 (Unix Timestamp)</param>
    /// <param name="ModDt">수정일자 (Unix Timestamp)</param>
    public sealed record SellerInfo(
        int Id,
        string HospName,
        string BusinessNo,
        string BusinessLevel,
        string HospNo,
        string ChartType,
        string BankCd,
        string BankName,
        string? BankImgPath,
        string DepositNo,
        string Depositor,
        bool Enabled,
        string? Etc,
        bool IsSync,
        int RegDt,
        int? ModDt
    );

    /// <summary>
    /// 판매자 송금 현황 집계 정보
    /// </summary>
    /// <param name="PendingAmount">대기 상태 송금 금액 합계</param>
    /// <param name="RequestAmount">요청 상태 송금 금액 합계</param>
    /// <param name="SuccessAmount">성공 상태 송금 금액 합계</param>
    /// <param name="FailAmount">실패 상태 송금 금액 합계</param>
    /// <param name="DeleteAmount">삭제(취소) 상태 송금 금액 합계</param>
    /// <param name="PendingCount">대기 상태 건수</param>
    /// <param name="RequestCount">요청 상태 건수</param>
    /// <param name="SuccessCount">성공 상태 건수</param>
    /// <param name="FailCount">실패 상태 건수</param>
    /// <param name="DeleteCount">삭제(취소) 상태 건수</param>
    public sealed record SellerRemitCountInfo(
        long PendingAmount,
        long RequestAmount,
        long SuccessAmount,
        long FailAmount,
        long DeleteAmount,
        int PendingCount,
        int RequestCount,
        int SuccessCount,
        int FailCount,
        int DeleteCount
    );
}
