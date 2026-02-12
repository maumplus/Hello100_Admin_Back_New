using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class EghisDoctInfoEntity
    {
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string EmplNo { get; set; }
        public string ClinicYmd { get; set; }
        public string DoctNo { get; set; }
        public string DoctNm { get; set; }
        public string DeptCd { get; set; }
        public string DeptNm { get; set; }
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
        public string Message { get; set; }
        public int Hello100Role { get; set; }
        public int Ridx { get; set; }
        public int ViewRole { get; set; }
        public string ViewMinTime { get; set; }
        public string ViewMinCnt { get; set; }
        public string UseYn { get; set; }
        public int UntactStartHour { get; set; }
        public int UntactStartMinute { get; set; }
        public int UntactEndHour { get; set; }
        public int UntactEndMinute { get; set; }
        public int UntactIntervalTime { get; set; }
        public string UntactUseYn { get; set; }
        public int UntactBreakStartHour { get; set; }
        public int UntactBreakStartMinute { get; set; }
        public int UntactBreakEndHour { get; set; }
        public int UntactBreakEndMinute { get; set; }
    }
}
