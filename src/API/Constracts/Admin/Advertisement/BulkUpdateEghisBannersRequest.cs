namespace Hello100Admin.API.Constracts.Admin.Advertisement
{
    public sealed record BulkUpdateEghisBannersRequest
    {
        /// <summary>
        /// 광고 ID
        /// </summary>
        public int AdId { get; init; }
        /// <summary>
        /// 노출여부
        /// </summary>
        public string ShowYn { get; init; } = default!;
        /// <summary>
        /// 정렬순서
        /// </summary>
        public int SortNo { get; init; }
    }
}
