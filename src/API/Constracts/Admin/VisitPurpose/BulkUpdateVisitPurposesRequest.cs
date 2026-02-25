namespace Hello100Admin.API.Constracts.Admin.VisitPurpose
{
    public record BulkUpdateVisitPurposesRequest
    {
        /// <summary>
        /// 요양기관키
        /// </summary>
        public required string HospKey { get; init; }
        /// <summary>
        /// 내원 목적 리스트
        /// </summary>
        public required List<BulkUpdateMyVisitPurposesRequest> VisitPurposes { get; init; }
    }

    public record BulkUpdateMyVisitPurposesRequest
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
