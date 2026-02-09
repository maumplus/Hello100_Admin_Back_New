namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser
{
    /// <summary>
    /// 사용자 정보 DTO
    /// </summary>
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tel { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string HospNo { get; set; } = string.Empty;
        public string AccountLocked { get; set; } = string.Empty;
        public string? LastLoginDt { get; set; }
        public string Use2fa { get; set; } = default!;
    }
}
