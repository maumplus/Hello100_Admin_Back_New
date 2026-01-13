using Hello100Admin.BuildingBlocks.Common.Application;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemitEnabled
{
    /// <summary>
    /// 판매자 송금 사용여부 수정 커맨드
    /// </summary>
    /// <param name="Id">일련번호</param>
    /// <param name="Enabled">활성 여부</param>
    public record UpdateSellerRemitEnabledCommand(int Id, bool Enabled) : ICommand<Result>;
}
