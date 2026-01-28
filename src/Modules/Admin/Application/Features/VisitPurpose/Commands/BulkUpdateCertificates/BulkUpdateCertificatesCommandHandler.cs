using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateCertificates
{
    public class BulkUpdateCertificatesCommandHandler : IRequestHandler<BulkUpdateCertificatesCommand, Result>
    {
        private readonly ILogger<BulkUpdateCertificatesCommandHandler> _logger;
        private readonly IVisitPurposeRepository _visitPurposeRepository;

        public BulkUpdateCertificatesCommandHandler(ILogger<BulkUpdateCertificatesCommandHandler> logger,
                                                    IVisitPurposeRepository visitPurposeRepository)
        {
            _logger = logger;
            _visitPurposeRepository = visitPurposeRepository;
        }

        public async Task<Result> Handle(BulkUpdateCertificatesCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Process BulkUpdateCertificatesCommandHandler started.");
            await _visitPurposeRepository.BulkUpdateCertificatesAsync(req, ct);

            return Result.Success();
        }
    }
}
