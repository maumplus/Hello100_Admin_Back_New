using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbEghisHospVisitPurposeInfoEntity
    {
        public string VpCd { get; set; }
        public string ParentCd { get; set; }
        public string HospKey { get; set; }
        public string InpuiryUrl { get; set; }
        public int InpuiryIdx { get; set; }
        public string InpuirySkipYn { get; set; }
        public string Name { get; set; }
        public string ShowYn { get; set; }
        public int SortNo { get; set; }
        public string DelYn { get; set; }
        public int Role { get; set; }
    }
}
