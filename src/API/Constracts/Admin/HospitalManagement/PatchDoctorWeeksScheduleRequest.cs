namespace Hello100Admin.API.Constracts.Admin.HospitalManagement
{
    public class PatchDoctorWeeksScheduleInfo
    {
        public string EmplNo { get; set; }
        public string ClinicYmd { get; set; }
        public int WeekNum { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public int BreakStartHour { get; set; }
        public int BreakStartMinute { get; set; }
        public int BreakEndHour { get; set; }
        public int BreakEndMinute { get; set; }
        public int IntervalTime { get; set; }
        public int Hello100Role { get; set; }
        public string UseYn { get; set; }
        public int Ridx { get; set; }
    }

    public class PatchDoctorWeeksScheduleRequest
    {
        public required string HospNo { get; set; }
        public required string HospKey { get; set; }
        public required string EmplNo { get; set; }
        public List<PatchDoctorWeeksScheduleInfo> DoctorScheduleList { get; set; }
    }

    public class PatchMyDoctorWeeksScheduleRequest
    {
        public required string EmplNo { get; set; }
        public List<PatchDoctorWeeksScheduleInfo> DoctorScheduleList { get; set; }
    }
}
