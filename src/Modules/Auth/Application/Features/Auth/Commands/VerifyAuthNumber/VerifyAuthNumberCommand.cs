using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.VerifyAuthNumber
{
    public record VerifyAuthNumberCommand : ICommand<Result>
    {
        /// <summary>
        /// 
        /// </summary>
        public string AppCd { get; set; } = "H02";
        /// <summary>
        /// 
        /// </summary>
        public int AuthId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AuthNumber { get; set; } = string.Empty;
    }
}
