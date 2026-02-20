using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class EghisDoctRsrvDetailInfoEntity
    {
        public int RsIdx { get; set; }
        public int Ridx { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int RsrvCnt { get; set; }
        public int ComCnt { get; set; }
        public string? RegDt { get; set; }
        public string ReceptType { get; set; }
    }
}
