using Hello100Admin.Modules.Admin.Application.Common.Models;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results
{
    public sealed class GetHospitalUserProfileResult
    {
        public string UId { get; set; } = default!;
        public int MId { get; set; }
        public string Name { get; set; } = default!;
        public string SnsId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string LoginType { get; set; } = default!;
        public string LoginTypeName { get; set; } = default!;
        public long Said { get; set; }
        public int RegDt { get; set; }
        public string RegDtView { get; set; } = default!;
        public int LastLoginDt { get; set; }
        public string? LastLoginDtView { get; set; }
        public string? LastLoginDtViewNew { get; set; }
        /// <summary>
        /// 사용자권한
        /// 0:일반사용자
        /// 1:테스트사용자
        /// </summary>
        public int UserRole { get; set; }
        public ListResult<GetHospitalUserProfileResultFamilyItem> Family { get; set; } = new ListResult<GetHospitalUserProfileResultFamilyItem>();
        public ListResult<GetHospitalUserProfileResultServiceUsageItem> ServiceUsages { get; set; } = new ListResult<GetHospitalUserProfileResultServiceUsageItem>();
    }

    public sealed class GetHospitalUserProfileResultFamilyItem
    {
        public string MId { get; set; } = default!;
        public string UId { get; set; } = default!;
        public string MemberNm { get; set; } = default!;
        public string Sex { get; set; } = default!;
        public string BirthDay { get; set; } = default!;
        public string RegDt { get; set; } = default!;
    }

    public sealed class GetHospitalUserProfileResultServiceUsageItem
    {
        public int RowNum { get; set; } = default!;
        public string ReqDate { get; set; } = default!;
        public int SerialNo { get; set; }
        public string Name { get; set; } = default!;
        public string ReceptType { get; set; } = default!;
        public string ReceptTypeNm { get; set; } = default!;
        public string PtntStateNm { get; set; } = default!;
        public int Amount { get; set; }
        public string? ProcessStatusNm { get; set; }
        public string DoctNm { get; set; } = default!;
        public string HospNo { get; set; } = default!;
        public string HospNm { get; set; } = default!;
    }
}
