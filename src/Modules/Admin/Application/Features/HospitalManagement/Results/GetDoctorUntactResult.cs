using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public class GetDoctorUntactResult
    {
        public string DoctNm { get; set; }
        public string EmplNo { get; set; }
        public string DoctIntro { get; set; }
        public string ClinicGuide { get; set; }
        public List<string> DoctHistoryList { get; set; }
    }
}
