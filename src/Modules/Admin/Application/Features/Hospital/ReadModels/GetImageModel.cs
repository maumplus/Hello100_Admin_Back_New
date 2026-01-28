using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Infrastructure.Persistence.DbModels.Hospital
{
    public class GetImageModel
    {
        public long ImgId { get; set; }
        public string ImgKey { get; set; }
        public string Url { get; set; }
        public char DelYn { get; set; }
        public string RegDt { get; set; }
    }
}
