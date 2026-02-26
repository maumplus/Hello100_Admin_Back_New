using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Asset.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Asset.Queries
{

    public record GetUsageListQuery : IQuery<Result<ListResult<GetUsageListResult>>>
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
        /// 미사용 경과일 시작일
        /// </summary>
        public string? FromDay { get; init; }

        /// <summary>
        /// 미사용 경과일 종료일
        /// </summary>
        public string? ToDay { get; init; }
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
    }

    public class GetUsageListQueryValidator : AbstractValidator<GetUsageListQuery>
    {
        public GetUsageListQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.SearchDateType).InclusiveBetween(1, 2).WithMessage("검색 날짜 조회 타입이 범위를 벗어났습니다.");
            RuleFor(x => x.SearchType).InclusiveBetween(1, 3).WithMessage("검색 키워드 조회 타입이 범위를 벗어났습니다.");
        }
    }

    public class GetUsageListQueryHandler : IRequestHandler<GetUsageListQuery, Result<ListResult<GetUsageListResult>>>
    {
        private readonly ILogger<GetUsageListQueryHandler> _logger;
        private readonly IAssetStore _assetStore;
        private readonly ICryptoService _cryptoService;

        public GetUsageListQueryHandler(
            ILogger<GetUsageListQueryHandler> logger,
            IAssetStore assetStore,
            ICryptoService cryptoService)
        {
            _logger = logger;
            _assetStore = assetStore;
            _cryptoService = cryptoService;
        }

        public async Task<Result<ListResult<GetUsageListResult>>> Handle(GetUsageListQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetUsageListQuery started.");

            var usageList = await _assetStore.GetUsageListAsync(
                req.PageSize, req.PageNo, req.SearchType, req.SearchDateType, req.FromDate, req.ToDate, req.FromDay, req.ToDay, req.SearchKeyword, false, ct);

            return Result.Success(usageList);
        }
    }

}
