namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbEghisHospMedicalTimeNewEntity
    {
        public string HospKey { get; set; }
        public string HospNo { get; set; }
        public int WeekNum { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public int BreakStartHour { get; set; }
        public int BreakStartMinute { get; set; }
        public int BreakEndHour { get; set; }
        public int BreakEndMinute { get; set; }
        public string UseYn { get; set; }
    }
}
