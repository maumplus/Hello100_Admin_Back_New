namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetVisitPurposeDetail
{
    public class GetVisitPurposeDetailResponse
    {
        public GetVisitPurposeDetailParentItemResponse Purpose { get; set; } = default!;
        public List<GetVisitPurposeDetailChildItemResponse> Details { get; set; } = new();
    }

    public class GetVisitPurposeDetailParentItemResponse
    {
        public short RequestApprYn { get; set; }
        public int CntPurpose { get; set; }
        public string? ChartType { get; set; }
        public string VpCd { get; set; } = default!;
        public string ParentCd { get; set; } = default!;
        public string HospKey { get; set; } = default!;
        public string InpuiryUrl { get; set; } = default!;
        public int InpuiryIdx { get; set; }
        public string PaperYn { get; set; } = default!;
        public string InpuirySkipYn { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ShowYn { get; set; } = default!;
        public Int16 SortNo { get; set; }
        public int Role { get; set; }
        public string DelYn { get; set; } = default!;
        public string RegDt { get; set; } = default!;
    }

    public class GetVisitPurposeDetailChildItemResponse
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
