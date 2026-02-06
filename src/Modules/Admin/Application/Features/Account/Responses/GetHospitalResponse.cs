namespace Hello100Admin.Modules.Admin.Application.Features.Account.Responses
{
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
    }
}
