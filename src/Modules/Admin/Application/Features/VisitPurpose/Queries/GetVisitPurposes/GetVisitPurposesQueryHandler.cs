using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetVisitPurposes;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetVisitPurposes
{
    public class GetVisitPurposesQueryHandler : IRequestHandler<GetVisitPurposesQuery, Result<GetVisitPurposesResponse>>
    {
        private readonly ILogger<GetVisitPurposesQueryHandler> _logger;
        private readonly IVisitPurposeStore _visitPurposeStore;

        public GetVisitPurposesQueryHandler(ILogger<GetVisitPurposesQueryHandler> logger, IVisitPurposeStore visitPurposeStore)
        {
            _logger = logger;
            _visitPurposeStore = visitPurposeStore;
        }

        public async Task<Result<GetVisitPurposesResponse>> Handle(GetVisitPurposesQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Process GetVisitPurposesQueryHandler started.");
            var visitPurposes = await _visitPurposeStore.GetVisitPurposesAsync(req.HospKey, ct);

            var response = visitPurposes.Adapt<GetVisitPurposesResponse>();

            return Result.Success(response);
        }
    }
}
