using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.ReadModels
{
    public class GetHospSettingModel
    {
        public string HospNo { get; set; }
        public string Name { get; set; }
        public int NoticeId { get; set; }
        public int StId { get; set; }
        public string HospKey { get; set; }
        public string WaitTm { get; set; }
        public string ReceptEndTime { get; set; }
        public string CallReceptEndTime { get; set; }
        public int Role { get; set; }
        public int AwaitRole { get; set; }
        public string Content { get; set; }
        public string RegDt { get; set; }
        public string SendYn { get; set; }
        public string SendStartYmd { get; set; }
        public string SendEndYmd { get; set; }
        public int ExamPushSet { get; set; } = 9;
        public string ExamApproveYn { get; set; } = "N";
    }
}
