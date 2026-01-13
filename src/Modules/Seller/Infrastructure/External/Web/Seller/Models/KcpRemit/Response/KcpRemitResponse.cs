namespace Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller.Models.KcpRemit.Response
{
    public class KcpRemitResponse
    {
        public string res_cd { get; set; }
        public string res_msg { get; set; }
        public string res_en_msg { get; set; }
        public string trade_seq { get; set; }
        public string trade_date { get; set; }
        public string amount { get; set; }
        public string bal_amount { get; set; }
        public string bankcode { get; set; }
        public string bankname { get; set; }
        public string account { get; set; }
        public string depositor { get; set; }
        public string app_time { get; set; }
        public string van_apptime { get; set; }
    }
}
