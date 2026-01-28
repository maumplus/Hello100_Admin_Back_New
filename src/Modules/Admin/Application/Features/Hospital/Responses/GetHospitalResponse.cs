using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.Responses
{
    public class MedicalTimeResponse
    {
        public long MtId { get; set; }
        public string HospKey { get; set; }
        public string MtNm { get; set; }
        public char DelYn { get; set; }
        public string RegDt { get; set; }
    }

    public class MedicalInfoResponse
    {
        public string MdCd { get; set; }
        public string HospKey { get; set; }
        public string MdNm { get; set; }
        public string RegDt { get; set; }
    }

    public class HashTagInfoResponse
    {
        public long TagId { get; set; }
        public string Kid { get; set; }
        public string HospKey { get; set; }
        public string TagNm { get; set; }
        public string Keyword { get; set; }
        public char DelYn { get; set; }
        public string RegDt { get; set; }
        public int MasterSeq { get; set; }
        public int DetailSeq { get; set; }
    }

    public class ImageInfoResponse
    {
        public long ImgId { get; set; }
        public string ImgKey { get; set; }
        public string Url { get; set; }
        public char DelYn { get; set; }
        public string RegDt { get; set; }
    }

    public class MedicalTimeNewResponse
    {
        public string HospKey { get; set; }
        public string HospNo { get; set; }
        public int WeekNum { get; set; }
        public string WeekNumNm { get; set; }
        public string StartHour { get; set; }
        public string StartMinute { get; set; }
        public string EndHour { get; set; }
        public string EndMinute { get; set; }
        public string BreakStartHour { get; set; }
        public string BreakStartMinute { get; set; }
        public string BreakEndHour { get; set; }
        public string BreakEndMinute { get; set; }
        public string UseYn { get; set; }
    }

    public class KeywordMasterResponse
    {
        public string Keyword { get; set; }
        public int MasterSeq { get; set; }
        public string MasterName { get; set; }
        public string DetailUseYn { get; set; }
        public string ShowYn { get; set; }
        public int SortNo { get; set; }
        public int SearchCnt { get; set; }
        public int DetailCnt { get; set; }
        public string RegDt { get; set; }
        public int DetailSeq { get; set; }
        public string TranSeq { get; set; }
    }

    public class GetHospitalResponse
    {
        public string HospKey { get; set; }
        public string HospNo { get; set; }
        public string BusinessNo { get; set; }
        public string Name { get; set; }
        public string HospClsCd { get; set; }
        public string Addr { get; set; }
        public string PostCd { get; set; }
        public string Tel { get; set; }
        public string Site { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public char ClosingYn { get; set; }
        public char DelYn { get; set; }
        public string Descrption { get; set; }
        public string RegDt { get; set; }
        public string ChartType { get; set; }
        public int IsTest { get; set; }
        public string MdCd { get; set; }
        public string MainMdCd { get; set; }
        public int KioskCnt { get; set; }
        public int TabletCnt { get; set; }
        public int RequestApprYn { get; set; }
        public List<MedicalTimeResponse> ClinicTimes { get; set; }
        public List<MedicalInfoResponse> DeptCodes { get; set; }
        public List<HashTagInfoResponse> Keywords { get; set; }
        public List<ImageInfoResponse> Images { get; set; }
        public List<MedicalTimeNewResponse> ClinicTimesNew { get; set; }
        public List<KeywordMasterResponse> KeywordMasters { get; set; }
    }
}
