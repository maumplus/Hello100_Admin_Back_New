using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Domain.Entities
{
    public class EghisRsrvInfoEntity
    {
        public string HospNo { get; set; }
        public string RsrvReceptNo { get; set; }
        public string ReceptNo { get; set; }
        public int SerialNo { get; set; }
        public string reqDate { get; set; }
        public string RsrvYmd { get; set; }
        public string RsrvTime { get; set; }
        public string AppUid { get; set; }
        public string DeptCd { get; set; }
        public string DeptNm { get; set; }
        public string PtntNo { get; set; }
        public string DoctEmplNo { get; set; }
        public int PtntState { get; set; }
        public int WaitSeq { get; set; }
        public string PtntNm { get; set; }
        public int ResultCd { get; set; }
        public string AllergyList { get; set; }
        public string RegDate { get; set; }
        public string ModDate { get; set; }
        public int TransYn { get; set; }
        public string Message { get; set; }
    }
}
