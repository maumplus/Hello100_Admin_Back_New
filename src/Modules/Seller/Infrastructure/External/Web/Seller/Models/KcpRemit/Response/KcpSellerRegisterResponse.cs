using Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller.Models.KcpRemit.Response.Common;

namespace Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller.Models.KcpRemit.Response
{
    public class KcpSellerRegisterResponse
    {
        public string res_cd { get; set; } = "";
        public string res_msg { get; set; } = "";
        public string res_en_msg { get; set; } = "";
        public List<KcpSellerInfo>? res_mall_info { get; set; }
    }
}
