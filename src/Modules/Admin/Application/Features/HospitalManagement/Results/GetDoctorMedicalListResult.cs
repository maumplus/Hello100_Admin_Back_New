using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public class DoctorMedicalInfo
    {
        public string MdCd { get; set; }
        public string HospKey { get; set; }
        public string MdNm { get; set; }
        public string RegDt { get; set; }
        public string CheckYn { get; set; }
    }

    public class GetDoctorMedicalListResult
    {
        public List<DoctorMedicalInfo> DoctorMedicalInfoList { get; set; }
    }
}
