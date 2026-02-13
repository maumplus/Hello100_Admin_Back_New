using Hello100Admin.Modules.Admin.Domain.Entities;
using System.Text.Json.Serialization;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PostDoctorUntactWeeksReservationRequest
    {
        [JsonIgnore]
        public string? HospNo { get; set; }
        public string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public int UntactRsrvIntervalTime { get; set; }
        [JsonIgnore]
        public int UntactRsrvIntervalCnt { get; set; } = 1;
        public string UntactAvaStartTime { get; set; }
        public string UntactAvaEndTime { get; set; }
        public string UntactAvaUseYn { get; set; }
        public List<EghisDoctRsrvDetailInfoEntity> EghisDoctRsrvDetailInfoList { get; set; }
    }
}
