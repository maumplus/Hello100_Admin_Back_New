using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class EghisDoctRsrvInfoEntity
    {
        public int Ridx { get; set; }
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
        public int WeekNum { get; set; }
        public string ClinicYmd { get; set; }
        public int RsrvIntervalTime { get; set; }
        public int RsrvIntervalCnt { get; set; }
        public string RegDt { get; set; }
        public int UntactRsrvIntervalTime { get; set; }
        public int UntactRsrvIntervalCnt { get; set; }
        public int UntactRsrvIntervalStartTime { get; set; }
        public int UntactRsrvIntervalEndTime { get; set; }
        public int UntactRsrvIntervalUseYn { get; set; }
    }
}
