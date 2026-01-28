using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateCertificates
{
    public record BulkUpdateCertificatesCommand : IQuery<Result>
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public List<BulkUpdateCertificatesCommandItem> Items { get; init; } = default!;
    }

    public record BulkUpdateCertificatesCommandItem
    {
        public string ReDocCd { get; set; } = default!;
        public char ShowYn { get; set; }
        public int SortNo { get; set; }
    }
}
