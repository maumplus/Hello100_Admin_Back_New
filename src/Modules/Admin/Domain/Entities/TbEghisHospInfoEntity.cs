using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbEghisHospInfoEntity
    {
        public string HospKey { get; set; }
        public string HospNo { get; set; }
        public string Description { get; set; }
        public string ClosingYn { get; set; }
        public string DelYn { get; set; }
        public string ChartType { get; set; }
    }
}
