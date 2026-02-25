using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public record GetSymptomExamKeywordDetailQuery(int MasterSeq) : IQuery<Result<GetSymptomExamKeywordDetailResult>>;

    public class GetSymptomExamKeywordDetailQueryValidator : AbstractValidator<GetSymptomExamKeywordDetailQuery>
    {
        public GetSymptomExamKeywordDetailQueryValidator()
        {
            RuleFor(x => x.MasterSeq).NotNull().GreaterThan(0).WithMessage("대표 키워드 고유번호는 필수이며, 0보다 커야합니다. ");
        }
    }

    public class GetSymptomExamKeywordDetailQueryHandler : IRequestHandler<GetSymptomExamKeywordDetailQuery, Result<GetSymptomExamKeywordDetailResult>>
    {
        private readonly ILogger<GetSymptomExamKeywordDetailQueryHandler> _logger;
        private readonly IHospitalManagementStore _hospitalManagementStore;
        private readonly IDbSessionRunner _db;

        public GetSymptomExamKeywordDetailQueryHandler(
            ILogger<GetSymptomExamKeywordDetailQueryHandler> logger,
            IHospitalManagementStore hospitalManagementStore,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementStore = hospitalManagementStore;
            _db = db;
        }
        public async Task<Result<GetSymptomExamKeywordDetailResult>> Handle(GetSymptomExamKeywordDetailQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetSymptomExamKeywordDetailQueryHandler for MasterSeq: {MasterSeq}", req.MasterSeq);

            var keywordDetail = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalManagementStore.GetSymptomExamKeywordDetailAsync(session, req.MasterSeq, token),
            ct);

            if (keywordDetail.Count <= 0)
                return Result.Success<GetSymptomExamKeywordDetailResult>().WithError(AdminErrorCode.NotFoundKeywordDetail.ToError());

            var masterInfo = keywordDetail.FirstOrDefault();

            var response = new GetSymptomExamKeywordDetailResult();
            response = masterInfo.Adapt<GetSymptomExamKeywordDetailResult>();
            response.DetailKeywordItems = keywordDetail.Adapt<List<GetSymptomExamKeywordDetailResultItem>>();

            return Result.Success(response);
        }
    }
}
