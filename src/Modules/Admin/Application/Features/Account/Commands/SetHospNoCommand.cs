using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Admin.Application.Features.Account.Commands
{
    public record SetHospNoCommand : IQuery<Result>
    {
        public string Aid { get; set; }
        public string HospNo { get; set; }
        public string HospKey { get; set; }
        public string ChartType { get; set; }
    }
}
