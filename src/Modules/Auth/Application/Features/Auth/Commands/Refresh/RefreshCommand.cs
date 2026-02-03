using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.Refresh;
using System.Text.Json.Serialization;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.Refresh
{
    public record RefreshCommand : ICommand<Result<RefreshResponse>>
    {
        [JsonIgnore]
        public string Aid { get; set; } = string.Empty;
        [JsonIgnore]
        public string? UserAgent { get; init; } = string.Empty;
        [JsonIgnore]
        public string? IpAddress { get; init; } = string.Empty;
    }
}
