namespace Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller.Models.KcpRemit.Request
{
    public class KcpRemitRequest
    {
        public string site_cd { get; set; }
        public string kcp_cert_info { get; set; }
        public string pay_method { get; set; } = "VCNT";
        public string cust_ip { get; set; }
        public string remit_type { get; set; } = "S";
        public string currency { get; set; } = "410";
        public string seller_id { get; set; }
        public string amount { get; set; }
        public string va_txtype { get; set; } = "48200000";
        public string va_name { get; set; } // 선택
        public string va_mny { get; set; }
    }
}
