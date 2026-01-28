using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ApprovalRequest;
using Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Responses;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Queries.GetUntactMedicalRequestsForApproval
{
    public class GetUntactMedicalRequestsForApprovalQueryHandler : IRequestHandler<GetUntactMedicalRequestsForApprovalQuery, Result<GetUntactMedicalRequestsForApprovalResponse>>
    {
        private readonly ILogger<GetUntactMedicalRequestsForApprovalQueryHandler> _logger;
        private readonly IApprovalRequestStore _approvalRequestStore;

        public GetUntactMedicalRequestsForApprovalQueryHandler(ILogger<GetUntactMedicalRequestsForApprovalQueryHandler> logger,
                                                               IApprovalRequestStore approvalRequestStore)
        {
            _approvalRequestStore = approvalRequestStore;
            _logger = logger;
        }

        public async Task<Result<GetUntactMedicalRequestsForApprovalResponse>> Handle(GetUntactMedicalRequestsForApprovalQuery req, CancellationToken ct)
        {
            _logger.LogInformation("GetUntactMedicalRequestsForApprovalQuery Started for HospKey: {HospKey}", req.HospKey);

            var list = await _approvalRequestStore.GetUntactMedicalRequestsForApprovalAsync(req.PageNo, req.PageSize, req.HospKey, req.ApprYn, ct);

            var response = list.Adapt<GetUntactMedicalRequestsForApprovalResponse>();

            return Result.Success(response);
        }
    }
}
