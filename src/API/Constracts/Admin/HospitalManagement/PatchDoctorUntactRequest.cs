using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PatchDoctorUntactRequest
    {
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public string DoctIntro { get; set; }
        public string ClinicGuide { get; set; }
        public List<string> DoctHistoryList { get; set; }
    }

    public class PatchMyDoctorUntactRequest
    {
        public string EmplNo { get; set; }
        public string DoctIntro { get; set; }
        public string ClinicGuide { get; set; }
        public List<string> DoctHistoryList { get; set; }
    }
}
