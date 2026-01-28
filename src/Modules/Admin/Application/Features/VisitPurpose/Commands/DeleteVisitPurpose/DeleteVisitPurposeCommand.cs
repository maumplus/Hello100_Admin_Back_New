using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.DeleteVisitPurpose
{
    public record DeleteVisitPurposeCommand : IQuery<Result>
    {
        /// <summary>
        /// 내원 키
        /// </summary>
        public string VpCd { get; init; } = default!;

        /// <summary>
        /// 요양기관 키
        /// </summary>
        public string HospKey { get; init; } = default!;
    }
}
