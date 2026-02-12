using System.Text.Json.Serialization;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class GetDoctorUntactWeeksReservationListRequest
    {
        [JsonIgnore]
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public string ReCalculateYn { get; set; } = "N";
        public string  UntactStartTime { get; set; }
        public string UntactEndTime { get; set; }
        public string UntactBreakStartTime { get; set; }
        public string UntactBreakEndTime { get; set; }
        public int UntactRsrvIntervalTime { get; set; }
        [JsonIgnore]
        public int UntactRsrvIntervalCnt { get; set; } = 1;
    }
}
