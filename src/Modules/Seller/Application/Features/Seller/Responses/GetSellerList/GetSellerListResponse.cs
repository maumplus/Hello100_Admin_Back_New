namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerList
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="RowNum">순번</param>
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
    /// <param name="RegAid">등록자 아이디</param>
    /// <param name="RegDt">등록일자 (Unix Timestamp)</param>
    /// <param name="ModDt">수정일자 (Unix Timestamp)</param>
    public sealed record GetSellerListResponse(
        int RowNum,
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
        string RegAid,
        int RegDt,
        int? ModDt
    );
}
