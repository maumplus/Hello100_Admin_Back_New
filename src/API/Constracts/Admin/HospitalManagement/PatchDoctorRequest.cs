using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PatchDoctorRequest
    {
        public string EmplNo { get; set; }
        public string DoctNm { get; set; }
        public string ViewMinCnt { get; set; }
        public string ViewMinTime { get; set; }
        public IFormFile Image { get; set; }
    }
}
