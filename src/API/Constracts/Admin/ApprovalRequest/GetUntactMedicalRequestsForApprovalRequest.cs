namespace Hello100Admin.API.Constracts.Admin.ApprovalRequest
{
    public class GetUntactMedicalRequestsForApprovalRequest
    {
        public required int PageNo { get; set; }
        public required int PageSize { get; set; }
        public required string ApprYn { get; set; }
    }
}
