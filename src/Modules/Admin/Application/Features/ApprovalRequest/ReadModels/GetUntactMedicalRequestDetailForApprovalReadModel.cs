namespace Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.ReadModels
{
    public sealed class GetUntactMedicalRequestDetailForApprovalReadModel
    {
        public string HospNm { get; set; }
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string EmplNo { get; set; }
        public string HospTel { get; set; }
        public string PostCd { get; set; }
        public string Addr { get; set; }
        public string DoctNo { get; set; }
        public string DoctNoType { get; set; }
        public string DoctNoTypeNm { get; set; }
        public int DoctLicenseFileSeq { get; set; }
        public string DoctLicenseFileSeqNm { get; set; }
        public string DoctNm { get; set; }
        public string DoctBirthday { get; set; }
        public string DoctTel { get; set; }
        public string DoctIntro { get; set; }
        public int DoctFileSeq { get; set; }
        public string DoctFileSeqNm { get; set; }
        public string DoctHistory { get; set; }
        public string ClinicTime { get; set; }
        public string ClinicGuide { get; set; }
        public int AccountInfoFileSeq { get; set; }
        public int BusinessFileSeq { get; set; }
        public string JoinState { get; set; }
        public string RegDt { get; set; }
        public string DoctFilePath { get; set; }
        public string BusinessFilePath { get; set; }
        public string LicenseFilePath { get; set; }
        public string AccountFilePath { get; set; }
        public string JoinStateNm { get; set; }
        public string ReturnReason { get; set; }
    }
}
