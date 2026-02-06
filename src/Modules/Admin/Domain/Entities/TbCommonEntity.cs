using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbCommonEntity
    {
        public int CmSeq { get; set; }
        public string ClsCd { get; set; }
        public string CmCd { get; set; }
        public string ClsName { get; set; }
        public string CmName { get; set; }
        public int Sort { get; set; }
        public string RegDt { get; set; }
    }
}
