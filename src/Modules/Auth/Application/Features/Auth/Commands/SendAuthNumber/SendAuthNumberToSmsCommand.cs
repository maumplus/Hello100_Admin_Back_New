using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.SendAuthNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.SendAuthNumber
{
    public record SendAuthNumberToSmsCommand : ICommand<Result<SendAuthNumberResponse>>
    {
        [JsonIgnore]
        public string AppCd { get; init; } = "H02";
        public string Tel { get; init; } = string.Empty;
    }
}
