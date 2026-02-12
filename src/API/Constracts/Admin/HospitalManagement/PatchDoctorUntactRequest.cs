using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PatchDoctorUntactRequest
    {
        public string EmplNo { get; set; }
        public string DoctIntro { get; set; }
        public string ClinicGuide { get; set; }
        public List<string> DoctHistoryList { get; set; }
    }
}
