using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Queries
{
    /// <summary>
    /// 문진표 목록 조회 쿼리
    /// </summary>
    /// <param name="HospNo"></param>
    /// <param name="HospKey"></param>
    public record GetQuestionnairesQuery(string HospNo, string HospKey) : IQuery<Result<ListResult<GetQuestionnairesResult>>>;

    public class GetQuestionnairesQueryValidator : AbstractValidator<GetQuestionnairesQuery>
    {
        public GetQuestionnairesQueryValidator()
        {
            RuleFor(x => x.HospNo)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");
        }
    }

    public class GetQuestionnairesQueryHandler : IRequestHandler<GetQuestionnairesQuery, Result<ListResult<GetQuestionnairesResult>>>
    {
        private readonly ILogger<GetQuestionnairesQueryHandler> _logger;
        private readonly IVisitPurposeStore _visitPurposeStore;
        private readonly IHospitalInfoProvider _hospitalInfoProvider;
        private readonly IDbSessionRunner _db;

        public GetQuestionnairesQueryHandler(
            ILogger<GetQuestionnairesQueryHandler> logger,
            IVisitPurposeStore visitPurposeStore,
            IHospitalInfoProvider hospitalInfoProvider,
            IDbSessionRunner db)
        {
            _logger = logger;
            _visitPurposeStore = visitPurposeStore;
            _hospitalInfoProvider = hospitalInfoProvider;
            _db = db;
        }

        public async Task<Result<ListResult<GetQuestionnairesResult>>> Handle(GetQuestionnairesQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetQuestionnairesQuery for HospNo: {HospNo} HospKey: {HospKey}", req.HospNo, req.HospKey);

            string hospNo = req.HospNo;

            if (string.IsNullOrWhiteSpace(hospNo) == true)
            {
                var hospInfo = await _hospitalInfoProvider.GetHospitalInfoByHospKeyAsync(req.HospKey, ct);

                hospNo = hospInfo?.HospNo ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(hospNo) == true)
                return Result.Success<ListResult<GetQuestionnairesResult>>().WithError(AdminErrorCode.NotFoundHospital.ToError());

            var result = await _db.RunAsync(DataSource.Hello100, 
                (session, token) => _visitPurposeStore.GetQuestionnairesAsync(session, hospNo, token)
            , ct);

            return Result.Success(result);
        }
    }
}
