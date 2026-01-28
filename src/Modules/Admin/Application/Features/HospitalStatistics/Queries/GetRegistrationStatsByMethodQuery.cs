using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalStatistics.Queries
{
    /// <summary>
    /// 접수구분통계 쿼리
    /// </summary>
    /// <param name="HospNo">요양기관번호</param>
    public record GetRegistrationStatsByMethodQuery(string HospNo, string year) : IQuery<Result<List<GetRegistrationStatsByMethodResult>>>;

    public class GetRegistrationStatsByMethodQueryValidator : AbstractValidator<GetRegistrationStatsByMethodQuery>
    {
        public GetRegistrationStatsByMethodQueryValidator()
        {
            RuleFor(x => x.HospNo)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");
            RuleFor(x => x.year)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("연도는 필수입니다.");
        }
    }

    public class GetRegistrationStatsByMethodQueryHandler : IRequestHandler<GetRegistrationStatsByMethodQuery, Result<List<GetRegistrationStatsByMethodResult>>>
    {
        private readonly ILogger<GetRegistrationStatsByMethodQueryHandler> _logger;
        private readonly IHospitalStatisticsStore _hospitalStatisticsStore;

        public GetRegistrationStatsByMethodQueryHandler(
            IHospitalStatisticsStore hospitalStatisticsStore,
            ILogger<GetRegistrationStatsByMethodQueryHandler> logger)
        {
            _hospitalStatisticsStore = hospitalStatisticsStore;
            _logger = logger;
        }
                  
        public async Task<Result<List<GetRegistrationStatsByMethodResult>>> Handle(GetRegistrationStatsByMethodQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetRegistrationStatsByMethodQuery for HospNo: {HospNo}, Year: {Year}", request.HospNo, request.year);

            var response = await _hospitalStatisticsStore.GetRegistrationStatsByMethodAsync(request.HospNo, request.year, cancellationToken);

            return Result.Success(response);
        }
    }
}
