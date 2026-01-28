using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Queries
{
    public record GetRegistrationStatsByVisitPurposeQuery(string HospNo, string yearMonth) : IQuery<Result<List<GetRegistrationStatsByVisitPurposeResult>>>;

    public class GetRegistrationStatsByVisitPurposeQueryValidator : AbstractValidator<GetRegistrationStatsByVisitPurposeQuery>
    {
        public GetRegistrationStatsByVisitPurposeQueryValidator()
        {
            RuleFor(x => x.HospNo)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");
            RuleFor(x => x.yearMonth)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("연도/월은 필수입니다.");
        }
    }

    public class GetRegistrationStatsByVisitPurposeQueryHandler : IRequestHandler<GetRegistrationStatsByVisitPurposeQuery, Result<List<GetRegistrationStatsByVisitPurposeResult>>>
    {
        private readonly ILogger<GetRegistrationStatsByVisitPurposeQueryHandler> _logger;
        private readonly IHospitalStatisticsStore _hospitalStatisticsStore;
        private readonly ICryptoService _cryptoService;

        public GetRegistrationStatsByVisitPurposeQueryHandler(
            IHospitalStatisticsStore hospitalStatisticsStore,
            ILogger<GetRegistrationStatsByVisitPurposeQueryHandler> logger,
            ICryptoService cryptoService)
        {
            _hospitalStatisticsStore = hospitalStatisticsStore;
            _logger = logger;
            _cryptoService = cryptoService;
        }

        public async Task<Result<List<GetRegistrationStatsByVisitPurposeResult>>> Handle(GetRegistrationStatsByVisitPurposeQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetRegistrationStatsByMethodQuery for HospNo: {HospNo}, Year: {Year}", request.HospNo, request.yearMonth);

            var response = await _hospitalStatisticsStore.GetRegistrationStatsByVisitPurposeAsync(request.HospNo, request.yearMonth, cancellationToken);

            if (response.Count > 0)
            {
                for (var i = 0; i < response.Count; i++)
                {
                    var purpose = _cryptoService.DecryptWithNoVector(response[i].VisitPurpose, CryptoKeyType.Mobile);
                    response[i].VisitPurpose = purpose;
                }
            }

            return Result.Success(response);
        }
    }
}
