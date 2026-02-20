using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Responses.GetVisitPurposeDetail;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries.GetVisitPurposeDetail
{
    public class GetVisitPurposeDetailQueryHandler : IRequestHandler<GetVisitPurposeDetailQuery, Result<GetVisitPurposeDetailResponse>>
    {
        private readonly IVisitPurposeStore _visitPurposeStore;
        private readonly ILogger<GetVisitPurposeDetailQueryHandler> _logger;

        public GetVisitPurposeDetailQueryHandler(IVisitPurposeStore visitPurposeStore,
                                                 ILogger<GetVisitPurposeDetailQueryHandler> logger)
        {
            _visitPurposeStore = visitPurposeStore;
            _logger = logger;
        }

        public async Task<Result<GetVisitPurposeDetailResponse>> Handle(GetVisitPurposeDetailQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Process GetVisitPurposeDetailQueryHandler started.");
            var detailInfo = await _visitPurposeStore.GetVisitPurposeDetailAsync(req, ct);

            if (detailInfo.Purpose == null)
            {
                _logger.LogWarning("No visit purpose detail found for VpCd: {VpCd}", req.VpCd);
                return Result.Success<GetVisitPurposeDetailResponse>().WithError(AdminErrorCode.NotFoundVisitPurpose.ToError());
            }

            var response = detailInfo.Adapt<GetVisitPurposeDetailResponse>();
            response.Purpose.PaperYn = response.Purpose.InpuiryIdx <= -1 ? "N" : "Y";

            return Result.Success(response);
        }
    }
}
