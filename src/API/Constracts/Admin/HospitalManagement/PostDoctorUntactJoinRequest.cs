using System.Text.Json.Serialization;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PostDoctorUntactJoinRequest
    {
        public string EmplNo { get; set; }
        public string HospNm { get; set; }
        public string HospTel { get; set; }
        public string PostCd { get; set; }
        public string DoctNo { get; set; }
        public string DoctNm { get; set; }
        public string DoctNoType { get; set; }
        public string DoctBirthday { get; set; }
        public string DoctTel { get; set; }
        public string DoctIntro { get; set; }
        public List<string> DoctHistoryList { get; set; }
        public string ClinicTime { get; set; }
        public string ClinicGuide { get; set; }
        public IFormFile DoctLicenseFile { get; set; }
        public IFormFile DoctFile { get; set; }
        public IFormFile AccountInfoFile { get; set; }
        public IFormFile BusinessFile { get; set; }
    }
}
