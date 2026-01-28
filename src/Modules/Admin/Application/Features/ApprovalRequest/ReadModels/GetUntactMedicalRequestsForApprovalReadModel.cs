namespace Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.ReadModels
{
    public class GetUntactMedicalRequestsForApprovalReadModel
    {
        public int TotalCount { get; set; }
        public List<GetUntactMedicalRequestsForApprovalItemReadModel> List { get; set; } = new();
    }

    public class GetUntactMedicalRequestsForApprovalItemReadModel
    {
        public int RowNum { get; set; }
        public int Seq { get; set; }
        public string JoinState { get; set; }
        public string JoinStateNm { get; set; }
        public string JoinStateModDt { get; set; }
        public string RegDt { get; set; }
        public string DoctNm { get; set; }
        public string DoctTel { get; set; }
        public string ModDt { get; set; }
        public string ReturnReason { get; set; }
    }
}
