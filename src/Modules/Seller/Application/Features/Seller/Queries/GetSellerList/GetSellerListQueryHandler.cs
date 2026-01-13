using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Common.Extensions;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.Shared;
using Mapster;
using MediatR;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerList
{
    public class GetSellerListQueryHandler : IRequestHandler<GetSellerListQuery, Result<PagedResult<GetSellerListResponse>>>
    {
        private ISellerStore _sellerStore;

        public GetSellerListQueryHandler(ISellerStore sellerStore)
        {
            _sellerStore = sellerStore;
        }

        public async Task<Result<PagedResult<GetSellerListResponse>>> Handle(GetSellerListQuery request, CancellationToken cancellationToken)
        {
            var sellerList = await _sellerStore.GetHospSellerListAsync(request, cancellationToken);

            // DB Error와 구분 필요
            if (sellerList == null || sellerList.Any() == false)
            {
                return Result.SuccessWithError<PagedResult<GetSellerListResponse>>(SellerErrorCode.NotFoundSeller.ToError());
            }

            var dtos = sellerList.Select(seller => new GetSellerListResponse
            (
                seller.RowNum,
                seller.Id,
                seller.HospName,
                seller.BusinessNo,
                seller.BusinessLevel,
                seller.HospNo,
                seller.ChartType,
                seller.BankCd,
                seller.BankName,
                seller.BankImgPath,
                seller.DepositNo,
                seller.Depositor,
                seller.Enabled == "1",
                seller.Etc,
                seller.IsSync == "1",
                seller.RegAid,
                seller.RegDt,
                seller.ModDt
            )).ToList();

            var result = new PagedResult<GetSellerListResponse>
            {
                Items = dtos,
                TotalCount = sellerList.FirstOrDefault()?.TotalCount ?? 0,
                Page = request.PageNo,
                PageSize = request.PageSize
            };

            return Result.Success(result);
        }
    }
}
