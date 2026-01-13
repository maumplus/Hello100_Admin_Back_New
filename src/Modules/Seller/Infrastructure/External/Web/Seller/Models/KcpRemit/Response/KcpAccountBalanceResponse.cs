namespace Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller.Models.KcpRemit.Response
{
    public class KcpAccountBalanceResponse
    {
        public string res_cd { get; set; }         // 결과 코드
        public string res_msg { get; set; }        // 결과 메시지
        public string res_en_msg { get; set; }     // 영문 메시지
        public string app_time { get; set; }       // 조회 시각
        public string can_amount { get; set; }     // 송금 가능 잔액
        public string bankcode { get; set; }       // 은행 코드
        public string depositor { get; set; }      // 예금주
        public string account { get; set; }        // 계좌번호 (마스킹됨)
    }
}
