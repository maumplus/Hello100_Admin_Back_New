using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetCertificates;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetCertificates
{
    public class GetCertificatesQueryHandler : IRequestHandler<GetCertificatesQuery, Result<GetCertificatesResponse>>
    {
        private readonly ILogger<GetCertificatesQueryHandler> _logger;
        private readonly IVisitPurposeStore _visitPurposeStore;

        public GetCertificatesQueryHandler(ILogger<GetCertificatesQueryHandler> logger,
                                           IVisitPurposeStore visitPurposeStore)
        {
            _logger = logger;
            _visitPurposeStore = visitPurposeStore;
        }

        public async Task<Result<GetCertificatesResponse>> Handle(GetCertificatesQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Process GetCertificatesQueryHandler started.");
            var certificates = await _visitPurposeStore.GetCertificatesAsync(req.HospKey, ct);

            var response = certificates.Adapt<GetCertificatesResponse>();

            return Result.Success(response);
        }
    }
}
