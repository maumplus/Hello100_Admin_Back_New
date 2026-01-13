namespace Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller.Models.KcpRemit.Request
{
    public class KcpSellerRegisterRequest
    {
        public string site_cd { get; set; }
        public string kcp_cert_info { get; set; }
        public string seller_id { get; set; }
        public string seller_name { get; set; }
        public string own_name { get; set; }
        public string address { get; set; }
        public string tel_no { get; set; }
        public string seller_level { get; set; }
        public string tax_no { get; set; }
        public string bank_cd { get; set; }
        public string deposit_no { get; set; }
        public string depositor { get; set; }
    }
}
