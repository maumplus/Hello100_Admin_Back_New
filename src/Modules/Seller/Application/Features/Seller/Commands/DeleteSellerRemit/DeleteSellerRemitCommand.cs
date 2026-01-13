using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.DeleteSellerRemit
{
    /// <summary>
    /// 판매자 송금정보 삭제 커맨드
    /// </summary>
    /// <param name="Id">송금 일련번호</param>
    /// <param name="Etc">비고</param>
    public record DeleteSellerRemitCommand(int Id, string? Etc) : ICommand<Result>;
}
