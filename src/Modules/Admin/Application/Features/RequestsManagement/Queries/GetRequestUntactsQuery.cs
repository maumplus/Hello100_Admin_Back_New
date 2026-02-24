

using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Queries
{
    public record GetRequestUntactsQuery : IQuery<Result<ListResult<GetRequestUntactsResult>>>
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
        /// 조회 시작일
        /// </summary>
        public string? FromDate { get; init; }

        /// <summary>
        /// 조회 종료일
        /// </summary>
        public string? ToDate { get; init; }
        /// <summary>
        /// 조회 타입 (1: 병원명, 2: 의사명, 3: 요양기관번호)
        /// </summary>
        public int SearchType { get; init; }
        /// <summary>
        /// 조회일 타입 (1: 전체, 2: 기간)
        /// </summary>
        public int SearchDateType { get; init; }
        /// <summary>
        /// 검색어
        /// </summary>
        public string? SearchKeyword { get; init; }
        /// <summary>
        /// 처리상태 (01: 신청, 02: 승인, 03: 반려)
        /// </summary>
        public List<string> JoinState { get; init; } = null!;
    }

    public class GetRequestUntactsQueryValidator : AbstractValidator<GetRequestUntactsQuery>
    {
        public GetRequestUntactsQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.SearchDateType).InclusiveBetween(1, 2).WithMessage("검색 날짜 조회 타입이 범위를 벗어났습니다.");
            RuleFor(x => x.SearchType).InclusiveBetween(1, 3).WithMessage("검색 키워드 조회 타입이 범위를 벗어났습니다.");
            RuleFor(x => x.JoinState).NotNull().WithMessage("처리 상태는 필수입니다.");
        }
    }

    public class GetRequestUntactsQueryHandler : IRequestHandler<GetRequestUntactsQuery, Result<ListResult<GetRequestUntactsResult>>>
    {
        private readonly ILogger<GetRequestUntactsQueryHandler> _logger;
        private readonly IRequestsManagementStore _requestsManagementStore;
        private readonly ICryptoService _cryptoService;

        public GetRequestUntactsQueryHandler(
            ILogger<GetRequestUntactsQueryHandler> logger, 
            IRequestsManagementStore requestsManagementStore, 
            ICryptoService cryptoService)
        {
            _logger = logger;
            _requestsManagementStore = requestsManagementStore;
            _cryptoService = cryptoService;
        }

        public async Task<Result<ListResult<GetRequestUntactsResult>>> Handle(GetRequestUntactsQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetRequestUntactsQuery started.");

            var requestUntactsList = await _requestsManagementStore.GetRequestUntactsAsync(
                req.PageSize, req.PageNo, req.SearchType, req.SearchDateType, req.FromDate, req.ToDate, req.SearchKeyword, req.JoinState, false, ct);

            return Result.Success(requestUntactsList);
        }
    }

}
