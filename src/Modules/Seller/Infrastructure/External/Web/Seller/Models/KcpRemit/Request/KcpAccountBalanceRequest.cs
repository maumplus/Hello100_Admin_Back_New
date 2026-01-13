namespace Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller.Models.KcpRemit.Request
{
    public class KcpAccountBalanceRequest
    {
        public string site_cd { get; set; }              // 사이트코드 (예: AO33D)
        public string kcp_cert_info { get; set; }        // 서비스 인증서
        public string pay_method { get; set; } = "VCNT"; // 결제수단 (고정값)
        public string amount { get; set; } = "0";        // 총 금액 (고정값)
        public string currency { get; set; } = "410";    // 화폐 단위 (고정값: 원화)
        public string va_txtype { get; set; } = "48100000"; // 요청 전문 유형 (고정값)
        public string cust_ip { get; set; }              // 요청자 IP
    }
}
