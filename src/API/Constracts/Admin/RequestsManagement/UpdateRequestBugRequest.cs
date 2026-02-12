namespace Hello100Admin.API.Constracts.Admin.RequestsManagement
{
    public sealed record UpdateRequestBugRequest
    {
        public required string ApprAId { get; set; }
    }
}
