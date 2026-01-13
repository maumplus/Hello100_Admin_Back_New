namespace Hello100Admin.API.Constracts.Seller
{
    public class UpdateSellerRemitRequest
    {
        /// <summary>
        /// 송금 요청 일련번호
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; set; }
    }
}
