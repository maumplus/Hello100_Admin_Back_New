using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbEghisDoctUntactJoinEntity
    {
        public string Seq { get; set; }
        public string HospNo { get; set; } = null!;
        public string HospKey { get; set; } = null!;
        public string HospNm { get; set; } = null!;
        public string EmplNo { get; set; } = null!;
        public string? HospTel { get; set; }
        public string PostCd { get; set; } = null!;
        public string DoctNo { get; set; } = null!;
        public string DoctNoType { get; set; } = null!;
        public int DoctLicenseFileSeq { get; set; }
        public string DoctNm { get; set; } = null!;
        public string? DoctBirthday { get; set; }
        public string DoctTel { get; set; } = null!;
        public string DoctIntro { get; set; } = null!;
        public int DoctTelSeq { get; set; }
        public string? DoctHistory { get; set; }
        public string? ClinicTime { get; set; }
        public string? ClinicGuide { get; set; }
        public int AccountInfoFileSeq { get; set; }
        public int BusinessFileSeq { get; set; }
        public string JoinState { get; set; } = null!;
        public int RegDt { get; set; }
        public int JoinStateModDt { get; set; }
        public string? ReturnReason { get; set; }
    }
}
