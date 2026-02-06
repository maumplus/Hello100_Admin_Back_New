using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateCertificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Account.Commands
{
    public record SetHospNoCommand : IQuery<Result>
    {
        [JsonIgnore]
        public string? Aid { get; set; }
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string ChartType { get; set; }
    }
}
