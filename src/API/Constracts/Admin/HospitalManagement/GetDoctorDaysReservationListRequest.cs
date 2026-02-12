using System.Text.Json.Serialization;

namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class GetDoctorDaysReservationListRequest
    {
        [JsonIgnore]
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public string ClinicYmd { get; set; }
        public string ReCalculateYn { get; set; } = "N";
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string BreakStartTime { get; set; }
        public string BreakEndTime { get; set; }
        public int RsrvIntervalTime { get; set; }
        public int RsrvIntervalCnt { get; set; }
    }
}
