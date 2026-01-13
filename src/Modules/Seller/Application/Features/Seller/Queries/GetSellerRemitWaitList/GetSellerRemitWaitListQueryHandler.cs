using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerRemitWaitList;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerRemitWaitList
{
    public class GetSellerRemitWaitListQueryHandler : IRequestHandler<GetSellerRemitWaitListQuery, Result<GetSellerRemitWaitListResponse>>
    {
        private readonly string? _imagePath;
        private readonly ILogger<GetSellerRemitWaitListQueryHandler> _logger;
        private readonly ISellerStore _sellerStore;

        public GetSellerRemitWaitListQueryHandler(IConfiguration config, 
                                                  ILogger<GetSellerRemitWaitListQueryHandler> logger,
                                                  ISellerStore sellerStore)
        {
            _imagePath = config["ApiImageUrl"];
            _logger = logger;
            _sellerStore = sellerStore;
        }

        public async Task<Result<GetSellerRemitWaitListResponse>> Handle(GetSellerRemitWaitListQuery req, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Processing get seller remit wait list");
            
            var remitWaitList = await _sellerStore.GetSellerRemitWaitListAsync(req.StartDt, req.EndDt, cancellationToken);
            
            foreach (var item in remitWaitList)
            {
                item.BankImgPath = $"{_imagePath}{item.BankImgPath}";
            }

            var result = remitWaitList.Adapt<List<GetSellerRemitWaitListInfo>>();

            var response = new GetSellerRemitWaitListResponse
            {
                List = result
            };

            return Result.Success(response);
        }
    }
}
