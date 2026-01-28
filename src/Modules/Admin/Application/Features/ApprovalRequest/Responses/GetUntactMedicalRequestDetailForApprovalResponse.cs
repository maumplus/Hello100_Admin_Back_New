namespace Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Responses
{
    public record GetUntactMedicalRequestDetailForApprovalResponse
    {
        public string HospNm { get; init; }
        public string HospNo { get; init; }
        public string HospKey { get; init; }
        public string EmplNo { get; init; }
        public string HospTel { get; init; }
        public string PostCd { get; init; }
        public string Addr { get; init; }
        public string DoctNo { get; init; }
        public string DoctNoType { get; init; }
        public string DoctNoTypeNm { get; init; }
        public int DoctLicenseFileSeq { get; init; }
        public string DoctLicenseFileSeqNm { get; init; }
        public string DoctNm { get; init; }
        public string DoctBirthday { get; init; }
        public string DoctTel { get; init; }
        public string DoctIntro { get; init; }
        public int DoctFileSeq { get; init; }
        public string DoctFileSeqNm { get; init; }
        public string DoctHistory { get; init; }
        public string ClinicTime { get; init; }
        public string ClinicGuide { get; init; }
        public int AccountInfoFileSeq { get; init; }
        public int BusinessFileSeq { get; init; }
        public string JoinState { get; init; }
        public string RegDt { get; init; }
        public string DoctFilePath { get; init; }
        public string BusinessFilePath { get; init; }
        public string LicenseFilePath { get; init; }
        public string AccountFilePath { get; init; }
        public string JoinStateNm { get; init; }
        public string ReturnReason { get; init; }
    }
}
