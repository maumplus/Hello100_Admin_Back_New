using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Common.Extensions;
using Hello100Admin.Modules.Seller.Application.Features.Seller.ReadModels.GetSellerDetail;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Responses.GetSellerDetail;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerDetail
{
    public class GetSellerDetailQueryHandler : IRequestHandler<GetSellerDetailQuery, Result<GetSellerDetailResponse>>
    {
        private readonly ISellerStore _sellerStore;
        private readonly string? _imagePath;

        public GetSellerDetailQueryHandler(IConfiguration config, ISellerStore sellerStore)
        {
            _imagePath = config["ApiImageUrl"];
            _sellerStore = sellerStore;
        }

        public async Task<Result<GetSellerDetailResponse>> Handle(GetSellerDetailQuery request, CancellationToken cancellationToken)
        {
            var hospSeller = await _sellerStore.GetHospSellerDetailInfoAsync(request.Id, cancellationToken);
            var remitInfo = await _sellerStore.GetSellerRemitCountAsync(request.Id, cancellationToken);

            if (hospSeller == null) 
                return Result.Success<GetSellerDetailResponse>().WithError(SellerErrorCode.NotFoundSeller.ToError());

            hospSeller.BankImgPath = $"{_imagePath}{hospSeller.BankImgPath}";

            // 추후 필요 시 Global setting으로 뺄 예정
            var config = new TypeAdapterConfig();
            config.NewConfig<GetHospSellerDetailInfoReadModel, SellerInfo>()
                .Map(d => d.IsSync, s => s.IsSync == "1")
                .Map(d => d.Enabled, s => s.Enabled == "1");

            var sellerInfo = hospSeller.Adapt<SellerInfo>(config);

            var remitCountInfo = remitInfo.Adapt<SellerRemitCountInfo>();

            var response = new GetSellerDetailResponse(sellerInfo, remitCountInfo);

            return Result.Success<GetSellerDetailResponse>(response);
        }
    }
}
