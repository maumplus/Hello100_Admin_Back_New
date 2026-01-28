using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.Hospital
{
    public class GetHospKeywordModel
    {
        public long TagId { get; set; }
        public string Kid { get; set; }
        public string HospKey { get; set; }
        public string TagNm { get; set; }
        public string Keyword { get; set; }
        public char DelYn { get; set; }
        public string RegDt { get; set; }
        public int MasterSeq { get; set; }
        public int DetailSeq { get; set; }
    }
}
