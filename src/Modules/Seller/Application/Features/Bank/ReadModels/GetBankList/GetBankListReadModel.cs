namespace Hello100Admin.Modules.Seller.Application.Features.Bank.ReadModels.GetBankList
{
    public class GetBankListReadModel
    {
        public int Id { get; set; }

        /// <summary>
        /// B:은행, C:증권
        /// </summary>
        public string Type { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Code { get; set; } = default!;

        public string? ImgPath { get; set; }
    }
}
