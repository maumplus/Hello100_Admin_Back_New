using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbFileInfoEntity
    {
        public string ClsCd { get; set; }
        public string CmCd { get; set; }
        public string FilePath { get; set; }
        public string OriginFileName { get; set; }
        public long FileSize { get; set; }
        public string DelYn { get; set; }
    }
}
