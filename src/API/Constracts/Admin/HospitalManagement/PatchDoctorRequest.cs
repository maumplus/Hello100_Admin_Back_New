using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PatchDoctorRequest
    {
        /// <summary>
        /// 의사사번
        /// </summary>
        public string EmplNo { get; set; }
        /// <summary>
        /// 의사명
        /// </summary>
        public string DoctNm { get; set; }
        /// <summary>
        /// 대기 시간표시에 따른 최소시간
        /// </summary>
        public string ViewMinTime { get; set; }
        /// <summary>
        /// 대기 인원표시에 따른 최소인원
        /// </summary>
        public string ViewMinCnt { get; set; }
        /// <summary>
        /// 의사사진
        /// </summary>
        public IFormFile? Image { get; set; }
    }
}
