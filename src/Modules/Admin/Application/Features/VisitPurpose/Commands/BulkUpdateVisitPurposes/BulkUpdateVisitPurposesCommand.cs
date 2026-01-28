using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateVisitPurposes
{
    public record BulkUpdateVisitPurposesCommand : IQuery<Result>
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 내원 목적 목록
        /// </summary>
        public List<BulkUpdateVisitPurposeCommandItem> Items { get; init; } = default!;
    }

    public record BulkUpdateVisitPurposeCommandItem
    {
        /// <summary>
        /// 정렬 번호
        /// </summary>
        public int SortNo { get; init; }

        /// <summary>
        /// 내원 키
        /// </summary>
        public string VpCd { get; init; } = default!;

        /// <summary>
        /// 노출 여부 (Y:필수, N:필수아님)
        /// </summary>
        public string ShowYn { get; init; } = default!;
    }
}
