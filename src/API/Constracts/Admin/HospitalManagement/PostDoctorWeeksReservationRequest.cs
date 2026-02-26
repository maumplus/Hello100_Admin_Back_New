using Hello100Admin.Modules.Admin.Domain.Entities;
using System.Text.Json.Serialization;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PostDoctorWeeksReservationRequest
    {
        public required string HospNo { get; set; }
        public required string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public int RsrvIntervalTime { get; set; }
        public int RsrvIntervalCnt { get; set; }
        public List<EghisDoctRsrvDetailInfoEntity> EghisDoctRsrvDetailInfoList { get; set; }
    }

    public class PostMyDoctorWeeksReservationRequest
    {
        public required string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public int RsrvIntervalTime { get; set; }
        public int RsrvIntervalCnt { get; set; }
        public List<EghisDoctRsrvDetailInfoEntity> EghisDoctRsrvDetailInfoList { get; set; }
    }
}
