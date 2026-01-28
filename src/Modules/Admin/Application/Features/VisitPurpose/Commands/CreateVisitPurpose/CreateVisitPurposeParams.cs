namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.CreateVisitPurpose
{
    #region 승인요청 테이블 저장을 위한 Params
    public class CreateVisitPurposeApprovalParams
    {
        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; set; } = default!;

        /// <summary>
        /// 승인유형 [HO: 병원 광고(1), HR: 병원 내원정보(2), HI: 병원정보(3)]
        /// </summary>
        public string ApprType { get; set; } = default!;

        /// <summary>
        /// 승인요청 데이터 (Json Serializing 된 string)
        /// </summary>
        public object Data { get; set; } = default!;

        /// <summary>
        /// 요청자 아이디
        /// </summary>
        public string ReqAid { get; set; } = default!;
    }
    #endregion

    #region 승인요청 테이블 Data Field (Json Serializing) 저장을 위한 Params
    public class CreateVisitPurposeParams
    {
        public CreateVisitPurposeBizParams Purpose { get; set; } = default!;
        public List<CreateVisitPurposeBizDetailsParams> Details { get; set; } = new();
        public List<CreateVisitPurposeBizPapersParams>? Papers { get; set; }
    }

    public class CreateVisitPurposeBizParams
    {
        public short RequestApprYn { get; set; } // 무조건 0
        public string TranId { get; set; } = ""; // 무조건 empty
        public string PaperYn { get; set; } = default!;
        public int CntPurpose { get; set; } // 무조건 0
        public string? ChartType { get; set; } // 무조건 null
        public string? VpCd { get; set; } // 무조건 null
        public string? ParentCd { get; set; } // 무조건 null
        public string HospKey { get; set; } = default!;
        public string InpuiryUrl { get; set; } = default!;
        public int InpuiryIdx { get; set; }
        public string InpuirySkipYn { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ShowYn { get; set; } = default!;
        public Int16 SortNo { get; set; } // 무조건 0
        public int Role { get; set; }
        public string DelYn { get; set; } = default!;
        public string RegDt { get; set; } = default!;
    }

    /// <summary>
    /// Name 빼고 불필요
    /// </summary>
    public class CreateVisitPurposeBizDetailsParams
    {
        public string? VpCd { get; set; }
        public string? ParentCd { get; set; }
        public string HospKey { get; set; } = default!;
        public string? InpuiryUrl { get; set; }
        public int InpuiryIdx { get; set; }
        public string? InpuirySkipYn { get; set; }
        public string Name { get; set; } = default!;
        public string? ShowYn { get; set; }
        public Int16 SortNo { get; set; }
        public int Role { get; set; }
        public string? DelYn { get; set; }
        public string? RegDt { get; set; }
    }

    /// <summary>
    /// 사용하지 않음
    /// </summary>
    public class CreateVisitPurposeBizPapersParams
    {
        public long IntCd { get; set; }
        public string? IntNm { get; set; }
    }
    #endregion
}
