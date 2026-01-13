using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Common.Errors;
using Hello100Admin.Modules.Seller.Application.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSellerRemit
{
    public class CreateSellerRemitCommandHandler : IRequestHandler<CreateSellerRemitCommand, Result>
    {
        private readonly ILogger<CreateSellerRemitCommandHandler> _logger;
        private readonly ISellerRepository _sellerRepository;

        public CreateSellerRemitCommandHandler(ILogger<CreateSellerRemitCommandHandler> logger, ISellerRepository sellerRepository)
        {
            _logger = logger;
            _sellerRepository = sellerRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> Handle(CreateSellerRemitCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Processing create seller remit AId: {aId}", command);

            var result = await _sellerRepository.InsertTbHospSellerRemitAsync(command, cancellationToken);

            if (result <= 0)
            {
                return Result.SuccessWithError(SellerErrorCode.SellerRemitInsertError.ToError());
            }

            return Result.Success();
        }
    }
}
