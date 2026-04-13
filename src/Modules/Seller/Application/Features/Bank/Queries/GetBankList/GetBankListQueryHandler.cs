using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Bank;
using Hello100Admin.Modules.Seller.Application.Features.Bank.Responses.GetBankList;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Bank.Queries.GetBankList
{
    public class GetBankListQueryHandler : IRequestHandler<GetBankListQuery, Result<GetBankListResponse>>
    {
        private readonly string? _imagePath;
        private readonly ILogger<GetBankListQueryHandler> _logger;
        private readonly IBankStore _bankStore;

        public GetBankListQueryHandler(IConfiguration config, ILogger<GetBankListQueryHandler> logger, IBankStore bankStore)
        {
            _imagePath = config["AdminImageUrl"];
            _logger = logger;
            _bankStore = bankStore;
        }

        public async Task<Result<GetBankListResponse>> Handle(GetBankListQuery req, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing get bank list start");

            var bankList = await _bankStore.GetBankListAsync(cancellationToken);

            // DB Error와 구분 필요
            if (bankList == null || !bankList.Any())
            {
                return Result.Success<GetBankListResponse>(new GetBankListResponse());
                //return Result.SuccessWithError<GetBankListResponse>(SellerErrorCode.NotFoundBankList.ToError());
            }

            var result = new GetBankListResponse
            {
                List = bankList.Select(b => new BankInfo
                {
                    Id = b.Id,
                    Type = b.Type,
                    Name = b.Name,
                    Code = b.Code,
                    Path = string.IsNullOrWhiteSpace(b.ImgPath) == false ? $"{_imagePath}Upload{b.ImgPath}" : ""
                }).ToList()
            };

            _logger.LogInformation("Processing get bank list end");

            return Result.Success(result);
        }
    }
}
