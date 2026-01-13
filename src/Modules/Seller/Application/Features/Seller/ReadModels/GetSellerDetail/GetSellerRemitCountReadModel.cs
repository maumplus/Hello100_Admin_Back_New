namespace Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerDetail
{
    public class GetSellerRemitCountReadModel
    {
        public int PendingAmount { get; set; }
        public int RequestAmount { get; set; }
        public int SuccessAmount { get; set; }
        public int FailAmount { get; set; }
        public int DeleteAmount { get; set; }
        public int PendingCount { get; set; }
        public int RequestCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public int DeleteCount { get; set; }
    }
}
