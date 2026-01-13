namespace Hello100Admin.API.Constracts.Seller
{
    public record UpdateSellerRemitEnabledRequest
    {
        /// <summary>
        /// 일련번호
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// 활성 여부
        /// </summary>
        public required bool Enabled { get; init; }
    }
}
