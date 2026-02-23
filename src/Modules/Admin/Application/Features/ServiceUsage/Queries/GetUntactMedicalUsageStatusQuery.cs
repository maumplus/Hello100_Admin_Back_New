using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries
{
    public record GetUntactMedicalUsageStatusQuery : IQuery<Result<GetUntactMedicalUsageStatusResult>>
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public int PageNo { get; init; }
        /// <summary>
        /// 페이지당 항목 수
        /// </summary>
        public int PageSize { get; init; }
        /// <summary>
        /// 조회 시작일 (yyyy-MM-dd)
        /// </summary>
        public string FromDate { get; init; } = default!;
        /// <summary>
        /// 조회 종료일 (yyyy-MM-dd)
        /// </summary>
        public string ToDate { get; init; } = default!;
        /// <summary>
        /// 조회 날짜 유형 [0: 당일, 1: 기간설정]
        /// </summary>
        public int SearchDateType { get; set; }
        /// <summary>
        /// 검색 유형 [1: 병원명, 2: 요양기관번호, 3: 회원명, 4: 주문번호]
        /// </summary>
        public int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
        /// <summary>
        /// 검색 상태 유형 (복수 선택 가능) [전체: "total", 접수완료: "recept", 진료완료: "end", 접수취소: "cancel"]
        /// </summary>
        public List<string> SearchStateTypes { get; init; } = default!;
        /// <summary>
        /// 검색 결제 유형 (복수 선택 가능) [전체: "total", 결제완료: "success", 결제실패: "fail"]
        /// </summary>
        public List<string> SearchPaymentTypes { get; init; } = default!;
    }

    public class GetUntactMedicalUsageStatusQueryValidator : AbstractValidator<GetUntactMedicalUsageStatusQuery>
    {
        public GetUntactMedicalUsageStatusQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.SearchType)
                .NotNull().GreaterThan(0).WithMessage("검색 유형은 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.FromDate)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("조회 시작일은 필수입니다.");
            RuleFor(x => x.ToDate)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("조회 종료일은 필수입니다.");
        }
    }

    public class GetUntactMedicalUsageStatusQueryHandler : IRequestHandler<GetUntactMedicalUsageStatusQuery, Result<GetUntactMedicalUsageStatusResult>>
    {
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly ILogger<GetUntactMedicalUsageStatusQueryHandler> _logger;
        private readonly IDbSessionRunner _db;

        public GetUntactMedicalUsageStatusQueryHandler(
            IServiceUsageStore serviceUsageStore, 
            ILogger<GetUntactMedicalUsageStatusQueryHandler> logger, 
            IDbSessionRunner db)
        {
            _serviceUsageStore = serviceUsageStore;
            _logger = logger;
            _db = db;
        }

        public async Task<Result<GetUntactMedicalUsageStatusResult>> Handle(GetUntactMedicalUsageStatusQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetUntactMedicalUsageStatusQuery");

            var result = await _db.RunAsync(DataSource.Hello100, 
                (session, token) => _serviceUsageStore.GetUntactMedicalUsageStatusAsync(session, req.PageNo, req.PageSize, req.FromDate, req.ToDate, req.SearchDateType, 
                req.SearchType, req.SearchKeyword, req.SearchStateTypes, req.SearchPaymentTypes, token),
            ct);

            return Result.Success(result);
        }
    }
}
