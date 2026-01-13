using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSeller
{
    /// <summary>
    /// 송금 등록 Command
    /// </summary>
    public record CreateSellerCommand
    (
        /// <summary>
        /// 관리자 AID
        /// </summary>
        string AId,
        /// <summary>
        /// 요양기관번호
        /// </summary>
        string HospNo,

        /// <summary>
        /// 은행 코드
        /// </summary>
        string BankCd,

        /// <summary>
        /// 계좌 번호 (하이픈 없이)
        /// </summary>
        string DepositNo,

        /// <summary>
        /// 예금주명
        /// </summary>
        string Depositor,

        /// <summary>
        /// 비고
        /// </summary>
        string? Etc
    ) : ICommand<Result>;
}
