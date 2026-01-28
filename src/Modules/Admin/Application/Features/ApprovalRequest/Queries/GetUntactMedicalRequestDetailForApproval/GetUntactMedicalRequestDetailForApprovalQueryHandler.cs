using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ApprovalRequest;
using Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Responses;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ApprovalRequest.Queries.GetUntactMedicalRequestDetailForApproval
{
    public class GetUntactMedicalRequestDetailForApprovalQueryHandler : IRequestHandler<GetUntactMedicalRequestDetailForApprovalQuery, Result<GetUntactMedicalRequestDetailForApprovalResponse>>
    {
        private readonly string _adminImageUrl;
        private readonly ILogger<GetUntactMedicalRequestDetailForApprovalQueryHandler> _logger;
        private readonly IApprovalRequestStore _approvalRequestStore;

        public GetUntactMedicalRequestDetailForApprovalQueryHandler(IConfiguration config,
                                                                    ILogger<GetUntactMedicalRequestDetailForApprovalQueryHandler> logger,
                                                                    IApprovalRequestStore approvalRequestStore)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _approvalRequestStore = approvalRequestStore;
            _logger = logger;
        }

        public async Task<Result<GetUntactMedicalRequestDetailForApprovalResponse>> Handle(GetUntactMedicalRequestDetailForApprovalQuery req, CancellationToken ct)
        {
            _logger.LogInformation("GetUntactMedicalRequestDetailForApprovalQuery Started for Seq: {Seq}", req.Seq);
            var detail = await _approvalRequestStore.GetUntactMedicalRequestDetailForApprovalAsync(req.Seq, _adminImageUrl, ct);

            var response = detail.Adapt<GetUntactMedicalRequestDetailForApprovalResponse>();

            return Result.Success(response);
        }
    }
}
