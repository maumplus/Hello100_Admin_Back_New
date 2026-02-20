namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PostDoctorUntactJoinRequest
    {
        public required string EmplNo { get; set; }
        public required string HospNm { get; set; }
        public required string HospTel { get; set; }
        public required string PostCd { get; set; }
        public required string DoctNo { get; set; }
        public required string DoctNm { get; set; }
        public required string DoctNoType { get; set; }
        public required string DoctBirthday { get; set; }
        public required string DoctTel { get; set; }
        public required string DoctIntro { get; set; }
        public required List<string> DoctHistoryList { get; set; }
        public string? ClinicTime { get; set; }
        public string? ClinicGuide { get; set; }
        public required IFormFile DoctLicenseFile { get; set; }
        public required IFormFile DoctFile { get; set; }
        public required IFormFile AccountInfoFile { get; set; }
        public required IFormFile BusinessFile { get; set; }
    }
}
