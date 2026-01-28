using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.ReadModels
{
    public class GetHospMedicalTimeModel
    {
        public long MtId { get; set; }
        public string HospKey { get; set; }
        public string MtNm { get; set; }
        public char DelYn { get; set; }
        public string RegDt { get; set; }
    }
}
