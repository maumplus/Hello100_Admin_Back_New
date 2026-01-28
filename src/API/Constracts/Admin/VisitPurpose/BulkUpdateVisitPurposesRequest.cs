namespace Hello100Admin.API.Constracts.Admin.VisitPurpose
{
    public record BulkUpdateVisitPurposesRequest
    {
        /// <summary>
        /// 정렬 번호
        /// </summary>
        public required int SortNo { get; init; }

        /// <summary>
        /// 내원 키
        /// </summary>
        public required string VpCd { get; init; }

        /// <summary>
        /// 노출 여부 (Y:필수, N:필수아님)
        /// </summary>
        public required string ShowYn { get; init; }
    }
}
