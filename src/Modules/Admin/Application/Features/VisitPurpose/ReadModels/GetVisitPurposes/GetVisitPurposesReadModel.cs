namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.ReadModels.GetVisitPurposes
{
    public sealed class GetVisitPurposesReadModel
    {
        public int TotalCount { get; set; }
        public List<GetVisitPurposesItemReadModel> DetailList { get; set; } = new();
    }

    public sealed class GetVisitPurposesItemReadModel
    {
        /// <summary>
        /// 내원 키
        /// </summary>
        public string VpCd { get; set; } = default!;

        /// <summary>
        /// 내원 부모 키
        /// </summary>
        public string ParentCd { get; set; } = default!;

        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; set; } = default!;

        /// <summary>
        /// 내원 명
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 노출 여부 (Y:필수, N:필수아님)
        /// </summary>
        public string ShowYn { get; set; } = default!;

        /// <summary>
        /// 정렬 순서
        /// </summary>
        public Int16 SortNo { get; set; }

        /// <summary>
        /// 등록 일시
        /// </summary>
        public string RegDt { get; set; } = default!;
    }
}
