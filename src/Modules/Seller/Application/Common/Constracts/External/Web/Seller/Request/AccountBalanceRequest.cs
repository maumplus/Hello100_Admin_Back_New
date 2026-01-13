namespace Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Request
{
    public class AccountBalanceRequest
    {
        public string SiteCd { get; set; }              // 사이트코드 (예: AO33D)
        public string KcpCertInfo { get; set; }        // 서비스 인증서
        public string PayMethod { get; set; } = "VCNT"; // 결제수단 (고정값)
        public string Amount { get; set; } = "0";        // 총 금액 (고정값)
        public string Currency { get; set; } = "410";    // 화폐 단위 (고정값: 원화)
        public string VaTxtype { get; set; } = "48100000"; // 요청 전문 유형 (고정값)
        public string CustIp { get; set; }              // 요청자 IP
    }
}
