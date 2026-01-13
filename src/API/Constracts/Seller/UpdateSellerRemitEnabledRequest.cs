namespace Hello100Admin.API.Constracts.Seller
{
    public class UpdateSellerRemitEnabledRequest
    {
        /// <summary>
        /// 일련번호
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// 활성 여부
        /// </summary>
        public required bool Enabled { get; set; }
    }
}
