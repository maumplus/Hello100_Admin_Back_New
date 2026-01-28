namespace Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Responses
{
    public sealed record GetUntactMedicalRequestsForApprovalResponse
    {
        public int TotalCount { get; init; }
        public List<GetUntactMedicalRequestsForApprovalItemResponse> List { get; init; } = new();
    }

    public sealed record GetUntactMedicalRequestsForApprovalItemResponse
    {
        public int RowNum { get; init; }
        public int Seq { get; init; }
        public string JoinState { get; init; }
        public string JoinStateNm { get; init; }
        public string JoinStateModDt { get; init; }
        public string RegDt { get; init; }
        public string DoctNm { get; init; }
        public string DoctTel { get; init; }
        public string ModDt { get; init; }
        public string ReturnReason { get; init; }
    }
}
