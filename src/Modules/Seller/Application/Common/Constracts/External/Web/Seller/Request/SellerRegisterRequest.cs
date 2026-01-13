namespace Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Request
{
    // 추후 변경 필요
    public class SellerRegisterRequest
    {
        public string SiteCd { get; set; } = "";
        public string KcpCertInfo { get; set; } = "";
        public string SellerId { get; set; } = "";
        public string SellerName { get; set; } = "";
        public string OwnName { get; set; } = "";
        public string Address { get; set; } = "";
        public string TelNo { get; set; } = "";
        public string SellerLevel { get; set; } = "";
        public string TaxNo { get; set; } = "";
        public string BankCd { get; set; } = "";
        public string DepositNo { get; set; } = "";
        public string Depositor { get; set; } = "";
    }
}
