namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.VisitPurpose
{
    internal sealed record GetVisitPurposeDetailChildRow
    {
        public string VpCd { get; set; } = default!;
        public string ParentCd { get; set; } = default!;
        public string HospKey { get; set; } = default!;
        public string InpuiryUrl { get; set; } = default!;
        public int InpuiryIdx { get; set; }
        public string InpuirySkipYn { get; set; } = "N";
        public string Name { get; set; } = default!;
        public string ShowYn { get; set; } = default!;
        public Int16 SortNo { get; set; }
        public int Role { get; set; }
        public string DelYn { get; set; } = "N";
        public string RegDt { get; set; } = default!;
    }
}
