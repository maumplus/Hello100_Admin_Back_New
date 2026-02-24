namespace Hello100Admin.API.Constracts.Admin.RequestsManagement
{

    public sealed record UpdateRequestUntactRequest
    {
        public required string JoinState { get; set; }
        public string? ReturnReason { get; set; }
    }
}
