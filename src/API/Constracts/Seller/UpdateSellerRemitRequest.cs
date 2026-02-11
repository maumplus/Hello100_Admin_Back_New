namespace Hello100Admin.API.Constracts.Seller
{
    public record UpdateSellerRemitRequest
    {
        /// <summary>
        /// 송금 요청 일련번호
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// 사용자 비밀번호
        /// </summary>
        public required string Password { get; init; }

        /// <summary>
        /// 비고
        /// </summary>
        public string? Etc { get; init; }
    }
}
