namespace Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Result
{
    public class RemitResult
    {
        public string ResCd { get; set; } = default!;
        public string ResMsg { get; set; } = default!;
        public string ResEnMsg { get; set; } = default!;
        public string TradeSeq { get; set; } = default!;
        public string TradeDate { get; set; } = default!;
        public long Amount { get; set; } = default!;
        public long BalAmount { get; set; } = default!;
        public string BankCode { get; set; } = default!;
        public string BankName { get; set; } = default!;
        public string Account { get; set; } = default!;
        public string Depositor { get; set; } = default!;
        public string AppTime { get; set; } = default!;
        public string VanApptime { get; set; } = default!;
    }
}
