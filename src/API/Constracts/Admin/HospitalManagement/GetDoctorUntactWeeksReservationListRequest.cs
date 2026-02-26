using System.Text.Json.Serialization;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class GetDoctorUntactWeeksReservationListRequest
    {
        public required string HospNo { get; set; }
        public required string EmplNo { get; set; }
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

    public class GetMyDoctorUntactWeeksReservationListRequest
    {
        public required string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public string ReCalculateYn { get; set; } = "N";
        public string UntactStartTime { get; set; }
        public string UntactEndTime { get; set; }
        public string UntactBreakStartTime { get; set; }
        public string UntactBreakEndTime { get; set; }
        public int UntactRsrvIntervalTime { get; set; }
        [JsonIgnore]
        public int UntactRsrvIntervalCnt { get; set; } = 1;
    }
}
