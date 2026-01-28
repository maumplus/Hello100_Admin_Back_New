namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetVisitPurposes
{
    public sealed class GetVisitPurposesResponse
    {
        public int TotalCount { get; init; }
        public List<GetVisitPurposesItem> DetailList { get; init; } = new();
    }

    public sealed class GetVisitPurposesItem
    {
        /// <summary>
        /// 내원 키 ('01': 공단검진)
        /// </summary>
        public string VpCd { get; init; } = default!;

        /// <summary>
        /// 내원 부모 키
        /// </summary>
        public string ParentCd { get; init; } = default!;

        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 내원 명
        /// </summary>
        public string Name { get; init; } = default!;

        /// <summary>
        /// 노출 여부 (Y:필수, N:필수아님)
        /// </summary>
        public string ShowYn { get; init; } = default!;

        /// <summary>
        /// 정렬 순서
        /// </summary>
        public Int16 SortNo { get; init; }

        /// <summary>
        /// 등록 일시
        /// </summary>
        public string RegDt { get; init; } = default!;
    }
}
