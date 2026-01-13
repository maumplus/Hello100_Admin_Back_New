using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemitEnabled
{
    public class UpdateSellerRemitEnabledCommandHandler : IRequestHandler<UpdateSellerRemitEnabledCommand, Result>
    {
        private readonly ILogger<UpdateSellerRemitEnabledCommandHandler> _logger;
        private readonly ISellerRepository _sellerRepository;

        public UpdateSellerRemitEnabledCommandHandler(ILogger<UpdateSellerRemitEnabledCommandHandler> logger,
                                                      ISellerRepository sellerRepository)
        {
            _logger = logger;
            _sellerRepository = sellerRepository;
        }

        public async Task<Result> Handle(UpdateSellerRemitEnabledCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Processing update seller remit enabled [{Id}] to [{Enabled}]", command.Id, command.Enabled);

            var updateCount = await _sellerRepository.UpdateSellerRemitEnabledAsync(command.Id, command.Enabled, cancellationToken);

            if (updateCount <= 0)
            {
                return Result.SuccessWithError(SellerErrorCode.UpdateSellerRemitEnabledFailedError.ToError());
            }

            _logger.LogInformation("Successfully updated seller remit enabled for Id [{Id}] to [{Enabled}]", command.Id, command.Enabled);

            return Result.Success();
        }
    }
}
