using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public record GetHospitalsUsingHello100ServiceQuery : IRequest<Result<ListResult<GetHospitalsUsingHello100ServiceResult>>>
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
        /// 검색차트타입 ["": 전체, E: 이지스전자차트, N: 닉스펜차트]
        /// </summary>
        public string? SearchChartType { get; init; }
        /// <summary>
        /// 검색 타입 [병원명: 1, 요양기관번호: 2]
        /// </summary>
        public int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
    }

    public class GetHospitalsUsingHello100ServiceQueryValidator : AbstractValidator<GetHospitalsUsingHello100ServiceQuery>
    {
        public GetHospitalsUsingHello100ServiceQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.SearchType).NotNull().GreaterThan(0).WithMessage("올바르지 않은 검색 타입입니다.");
        }
    }

    public class GetHospitalsUsingHello100ServiceQueryHandler : IRequestHandler<GetHospitalsUsingHello100ServiceQuery, Result<ListResult<GetHospitalsUsingHello100ServiceResult>>>
    {
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetHospitalsUsingHello100ServiceQueryHandler> _logger;
        private readonly IDbSessionRunner _db;

        public GetHospitalsUsingHello100ServiceQueryHandler(
            IHospitalManagementStore hospitalStore,
            ILogger<GetHospitalsUsingHello100ServiceQueryHandler> logger,
            IDbSessionRunner db)
        {
            _hospitalStore = hospitalStore;
            _logger = logger;
            _db = db;
        }

        public async Task<Result<ListResult<GetHospitalsUsingHello100ServiceResult>>> Handle(GetHospitalsUsingHello100ServiceQuery req, CancellationToken ct)
        {
           _logger.LogInformation("Handling GetHospitalsUsingHello100ServiceQueryQuery");

            var response = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalStore.GetHospitalsUsingHello100ServiceAsync(session, req.PageNo, req.PageSize, req.SearchChartType, req.SearchType, req.SearchKeyword, token), 
            ct);

            return Result.Success(response);
        }
    }
}
