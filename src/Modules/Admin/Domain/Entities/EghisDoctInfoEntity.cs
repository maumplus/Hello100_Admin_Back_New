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
        public string DoctNm { get; set; }
        public int ViewMinTime { get; set; }
        public int ViewMinCnt { get; set; }
    }
}
