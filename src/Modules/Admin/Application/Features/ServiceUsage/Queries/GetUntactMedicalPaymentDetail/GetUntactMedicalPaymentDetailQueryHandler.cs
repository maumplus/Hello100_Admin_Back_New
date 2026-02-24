using System.Globalization;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetUntactMedicalPaymentDetail;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetUntactMedicalPaymentDetail
{
    public class GetUntactMedicalPaymentDetailQueryHandler : IRequestHandler<GetUntactMedicalPaymentDetailQuery, Result<GetUntactMedicalPaymentDetailResponse>>
    {
        private readonly ILogger<GetUntactMedicalPaymentDetailQueryHandler> _logger;
        private readonly IServiceUsageStore _serviceUsageStore;

        public GetUntactMedicalPaymentDetailQueryHandler(ILogger<GetUntactMedicalPaymentDetailQueryHandler> logger, IServiceUsageStore serviceUsageStore)
        {
            _logger = logger;
            _serviceUsageStore = serviceUsageStore;
        }

        public async Task<Result<GetUntactMedicalPaymentDetailResponse>> Handle(GetUntactMedicalPaymentDetailQuery req, CancellationToken token)
        {
            _logger.LogInformation("Process GetUntactMedicalPaymentDetailQuery() started.");
            var paymentDetail = await _serviceUsageStore.GetUntactMedicalPaymentDetailAsync(req.PaymentId, token);

            if (paymentDetail == null)
                return Result.Success<GetUntactMedicalPaymentDetailResponse>().WithError(AdminErrorCode.NotFoundUntactMedicalPayment.ToError());

            paymentDetail.CardNo = paymentDetail.CardNo?.Length == 16 ? $"{paymentDetail.CardNo[..4]}-****-****-{paymentDetail.CardNo[12..16]}" : paymentDetail.CardNo;

            var tempAppTime = "-";

            if (!string.IsNullOrEmpty(paymentDetail.AppTime) &&
                DateTime.TryParseExact(paymentDetail.AppTime, "yyyyMMdd", null, DateTimeStyles.None, out var dt))
            {
                tempAppTime = dt.ToString("yyyy.MM.dd");
            }

            paymentDetail.AppTime = tempAppTime;

            var result = paymentDetail.Adapt<GetUntactMedicalPaymentDetailResponse>();

            return Result.Success(result);
        }
    }
}
