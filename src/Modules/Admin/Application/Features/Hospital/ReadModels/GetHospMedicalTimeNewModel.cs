using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.ReadModels
{
    public class GetHospMedicalTimeNewModel
    {
        public string HospKey { get; set; }
        public string HospNo { get; set; }
        public int WeekNum { get; set; }
        public string WeekNumNm { get; set; }
        public string StartHour { get; set; }
        public string StartMinute { get; set; }
        public string EndHour { get; set; }
        public string EndMinute { get; set; }
        public string BreakStartHour { get; set; }
        public string BreakStartMinute { get; set; }
        public string BreakEndHour { get; set; }
        public string BreakEndMinute { get; set; }
        public string UseYn { get; set; }
    }
}
