using Hello100Admin.Modules.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results
{
    public class GetDoctorDaysReservationListResult
    {
        public EghisDoctRsrvInfoEntity? EghisDoctRsrvInfo { get; set; }
        public List<EghisDoctRsrvDetailInfoEntity> EghisDoctRsrvDetailInfoList { get; set; }
        public List<EghisRsrvInfoEntity> EghisRsrvList { get; set; }
    }
}
