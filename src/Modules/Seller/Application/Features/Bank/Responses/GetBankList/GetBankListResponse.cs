namespace Hello100Admin.Modules.Seller.Application.Features.Bank.Responses.GetBankList
{
    public record GetBankListResponse
    {
        public IList<BankInfo> List { get; set; } = new List<BankInfo>();
    }

    public class BankInfo
    {
        /// <summary>
        /// 일련번호
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 구분 (B:은행, C:증권)
        /// </summary>
        public string Type { get; set; } = default!;

        /// <summary>
        /// 이름
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 코드
        /// </summary>
        public string Code { get; set; } = default!;

        /// <summary>
        /// 이미지 경로
        /// </summary>
        public string Path { get; set; } = default!;
    }
}
