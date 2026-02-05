namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public sealed class GetDoctorListResult
    {
        public string HospNo { get; set; } = string.Empty;
        public string HospKey { get; set; } = string.Empty;
        public string EmplNo { get; set; } = string.Empty;
        public string DoctNo { get; set; } = string.Empty;
        public string DoctNm { get; set; } = string.Empty;
        public string DeptCd { get; set; } = string.Empty;
        public string DeptNm { get; set; } = string.Empty;
        public string WeeksNm { get; set; } = string.Empty;
        public string FrontViewRole { get; set; } = string.Empty;
    }
}
