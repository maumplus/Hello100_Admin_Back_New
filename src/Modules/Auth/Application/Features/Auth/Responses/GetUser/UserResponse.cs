namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser
{
    /// <summary>
    /// 사용자 정보 DTO
    /// </summary>
    public class UserResponse
    {
        public string Aid { get; set; } = string.Empty;
        public string AccId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? HospNo { get; set; }
        public string? HospKey { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string AccountLocked { get; set; } = string.Empty;
        public string? LastLoginDt { get; set; }
    }
}
