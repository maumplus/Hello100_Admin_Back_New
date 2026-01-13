using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Common.Extensions;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerRemitList;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.Shared;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerRemitList
{
    public class GetSellerRemitListQueryHandler : IRequestHandler<GetSellerRemitListQuery, Result<PagedResult<GetSellerRemitListResponse>>>
    {
        private readonly string? _imagePath;
        private readonly ISellerStore _sellerStore;
        private readonly ILogger<GetSellerRemitListQueryHandler> _logger;

        public GetSellerRemitListQueryHandler(IConfiguration config, ILogger<GetSellerRemitListQueryHandler> logger, ISellerStore sellerStore)
        {
            _imagePath = config["ApiImageUrl"];
            _sellerStore = sellerStore;
            _logger = logger;
        }

        public async Task<Result<PagedResult<GetSellerRemitListResponse>>> Handle(GetSellerRemitListQuery req, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Processing get seller remit list");

            var remitList = await _sellerStore.GetHospSellerRemitListAsync(req, cancellationToken);

            // DB Error와 구분 필요
            if (remitList == null || remitList.Any() == false)
            {
                var failedResult =  new PagedResult<GetSellerRemitListResponse>
                {
                    Items = new List<GetSellerRemitListResponse>(),
                    TotalCount = 0,
                    Page = req.PageNo,
                    PageSize = req.PageSize
                };

                return Result.Success(failedResult);
            }

            foreach (var item in remitList)
            {
                item.BankImgPath = $"{_imagePath}{item.BankImgPath}";
            }

            var dtos = remitList.Adapt<List<GetSellerRemitListResponse>>();

            var result = new PagedResult<GetSellerRemitListResponse>
            {
                Items = dtos,
                TotalCount = remitList.FirstOrDefault()?.TotalCount ?? 0,
                Page = req.PageNo,
                PageSize = req.PageSize
            };

            return Result.Success(result);
        }
    }
}
