namespace Hello100Admin.Modules.Seller.Infrastructure.Persistence.DbModels.Bank
{
    internal sealed record GetBankListRow
    {
        public int Id { get; init; }

        /// <summary>
        /// B:은행, C:증권
        /// </summary>
        public string Type { get; init; } = null!;

        public string Name { get; init; } = null!;

        public string Code { get; init; } = null!;

        public string? ImgPath { get; init; }
    }
}
