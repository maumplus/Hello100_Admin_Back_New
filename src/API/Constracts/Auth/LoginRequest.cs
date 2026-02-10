using System.Text.Json.Serialization;

namespace Hello100Admin.API.Constracts.Auth
{
    public class LoginRequest
    {
        public string AccountId { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        [JsonIgnore]
        public string AppCd { get; set; } = "H02";
        public int AuthId { get; set; }
        public string AuthNumber { get; set; } = string.Empty;
        [JsonIgnore]
        public string? UserAgent { get; init; } = string.Empty;
        [JsonIgnore]
        public string? IpAddress { get; init; } = string.Empty;
    }
}
