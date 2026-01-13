namespace Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Request
{
    public class RemitRequest
    {
        public string SiteCd { get; set; }
        public string KcpCertInfo { get; set; }
        public string PayMethod { get; set; } = "VCNT";
        public string CustIp { get; set; }

        public string RemitType { get; set; } = "S";
        public string Currency { get; set; } = "410";
        public string SellerId { get; set; }
        public string Amount { get; set; }
        public string VaTxtype { get; set; } = "48200000";
        public string VaName { get; set; } // 선택
        public string VaMny { get; set; }
    }
}
