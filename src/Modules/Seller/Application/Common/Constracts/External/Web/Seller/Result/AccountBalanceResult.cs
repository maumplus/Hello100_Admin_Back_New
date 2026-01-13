namespace Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Result
{
    public class AccountBalanceResult
    {
        public string ResCd { get; set; }         // 결과 코드
        public string ResMsg { get; set; }        // 결과 메시지
        public string ResEnMsg { get; set; }     // 영문 메시지
        public string AppTime { get; set; }       // 조회 시각
        public string CanAmount { get; set; }     // 송금 가능 잔액
        public string BankCode { get; set; }       // 은행 코드
        public string Depositor { get; set; }      // 예금주
        public string Account { get; set; }        // 계좌번호 (마스킹됨)
    }
}
