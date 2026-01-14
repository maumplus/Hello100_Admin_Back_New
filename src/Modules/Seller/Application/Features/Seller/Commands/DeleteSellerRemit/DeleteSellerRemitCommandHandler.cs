using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.DeleteSellerRemit
{
    public class DeleteSellerRemitCommandHandler : IRequestHandler<DeleteSellerRemitCommand, Result>
    {
        private readonly ILogger<DeleteSellerRemitCommandHandler> _logger;
        private readonly ISellerRepository _sellerRepository;

        public DeleteSellerRemitCommandHandler(ILogger<DeleteSellerRemitCommandHandler> logger,
                                               ISellerRepository sellerRepository)
        {
            this._logger = logger;
            _sellerRepository = sellerRepository;
        }

        public async Task<Result> Handle(DeleteSellerRemitCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Processing delete seller remit");

            var deleteCount = await _sellerRepository.DeleteSellerRemitAsync(command.Id, command.Etc, cancellationToken);

            switch (deleteCount)
            {
                case 0:
                    return Result.Success().WithError(SellerErrorCode.SellerRemitDeleteFailed.ToError());
                case 1:
                    break;
                case -2:
                    return Result.Success().WithError(SellerErrorCode.SellerRemitAlreadyCompleted.ToError());
                case -3:
                    return Result.Success().WithError(SellerErrorCode.SellerRemitAlreadyDeleted.ToError());
                case -5:
                    return Result.Success().WithError(SellerErrorCode.SellerRemitDeleteFailedError.ToError());
                default:
                    _logger.LogError("DeleteSellerRemitAsync returned unexpected deleteCount: {DeleteCount}", deleteCount);
                    break;
            }

            return Result.Success();
        }
    }
}
