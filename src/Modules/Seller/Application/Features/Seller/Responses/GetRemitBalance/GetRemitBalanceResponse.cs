namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetRemitBalance
{
    /// <summary>
    /// 송금 잔액 조회 응답 DTO
    /// </summary>
    /// <param name="ResCd">결과 코드 (0000: 성공)</param>
    /// <param name="ResMsg">결과 메시지 (한글)</param>
    /// <param name="ResEnMsg">결과 메시지 (영문)</param>
    /// <param name="AppTime">응답 수신 시각</param>
    /// <param name="BankCode">입금 은행 코드</param>
    /// <param name="Account">입금 계좌번호 (마스킹)</param>
    /// <param name="Depositor">예금주명</param>
    /// <param name="CanAmount">송금 가능 금액 (원화 단위)</param>
    public sealed record GetRemitBalanceResponse(
        string ResCd,
        string ResMsg,
        string ResEnMsg,
        string AppTime,
        string BankCode,
        string Account,
        string Depositor,
        long CanAmount = 0
    );
}
