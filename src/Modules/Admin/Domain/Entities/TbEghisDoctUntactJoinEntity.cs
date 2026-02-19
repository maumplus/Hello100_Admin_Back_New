using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class TbEghisDoctUntactJoinEntity
    {
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string HospNm { get; set; }
        public string EmplNo { get; set; }
        public string HospTel { get; set; }
        public string PostCd { get; set; }
        public string DoctNo { get; set; }
        public string DoctNoType { get; set; }
        public int DoctLicenseFileSeq { get; set; }
        public TbFileInfoEntity? DoctLicenseFileInfo { get; set; }
        public string DoctNm { get; set; }
        public string DoctBirthday { get; set; }
        public string DoctTel { get; set; }
        public string DoctIntro { get; set; }
        public int DoctFileSeq { get; set; }
        public TbFileInfoEntity? DoctFileSeqInfo { get; set; }
        public string DoctHistory { get; set; }
        public string ClinicTime { get; set; }
        public string ClinicGuide { get; set; }
        public int AccountInfoFileSeq { get; set; }
        public TbFileInfoEntity? AccountInfoFileInfo { get; set; }
        public int BusinessFileSeq { get; set; }
        public TbFileInfoEntity? BusinessFileInfo { get; set; }
        public string JoinState { get; set; }
        public string RegDt { get; set; }
    }
}
