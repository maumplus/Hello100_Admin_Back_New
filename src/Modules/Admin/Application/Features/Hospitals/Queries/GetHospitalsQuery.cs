using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Hospitals.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospitals.Queries
{
    public record SearchHospitalsQuery : IQuery<Result<ListResult<SearchHospitalsResult>>>
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public int PageNo { get; init; }

        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// 검색 타입 [0: 병원명, 1: 대표번호]
        /// </summary>
        public int SearchType { get; init; }

        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
    }

    public class SearchHospitalsQueryValidator : AbstractValidator<SearchHospitalsQuery>
    {
        public SearchHospitalsQueryValidator()
        {
            RuleFor(x => x.PageNo)
                .NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize)
                .NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
        }
    }

    public class SearchHospitalsQueryHandler : IRequestHandler<SearchHospitalsQuery, Result<ListResult<SearchHospitalsResult>>>
    {
        private readonly ILogger<SearchHospitalsQueryHandler> _logger;
        private readonly IHospitalsStore _hospitalsStore;
        private readonly IDbSessionRunner _db;

        public SearchHospitalsQueryHandler(ILogger<SearchHospitalsQueryHandler> logger, IHospitalsStore hospitalsStore, IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalsStore = hospitalsStore;
            _db = db;
        }

        public async Task<Result<ListResult<SearchHospitalsResult>>> Handle(SearchHospitalsQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handle SearchHospitalsQueryHandler");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalsStore.SearchHospitalsAsync(session, req.PageNo, req.PageSize, req.SearchType, req.SearchKeyword, token),
            ct);

            return Result.Success(result);
        }
    }
}
