using System.Text.Json.Serialization;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class GetDoctorWeeksReservationListRequest
    {
        public required string HospNo { get; set; }
        public required string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public string ReCalculateYn { get; set; } = "N";
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string BreakStartTime { get; set; }
        public string BreakEndTime { get; set; }
        public int RsrvIntervalTime { get; set; }
        public int RsrvIntervalCnt { get; set; }
    }

    public class GetMyDoctorWeeksReservationListRequest
    {
        public required string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public string ReCalculateYn { get; set; } = "N";
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string BreakStartTime { get; set; }
        public string BreakEndTime { get; set; }
        public int RsrvIntervalTime { get; set; }
        public int RsrvIntervalCnt { get; set; }
    }
}
