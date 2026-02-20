using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Queries
{
    public record GetHospitalAdminListQuery : IQuery<Result<ListResult<GetHospitalAdminListResult>>>
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
        /// QR코드 생성 상태 [전체: 0, 매칭완료: 1, QR발행: 2, QR미발행: 3, 대리점 배정: 4, 대리점 미배정: 5]
        /// </summary>
        public int QrState { get; init; }
        /// <summary>
        /// 검색 타입 [병원명: 1, 아이디: 2, 대리점: 3]
        /// </summary>
        public int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
    }

    public class GetHospitalAdminListQueryValidator : AbstractValidator<GetHospitalAdminListQuery>
    {
        public GetHospitalAdminListQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.QrState)
                .NotNull().InclusiveBetween(0, 5).WithMessage("QR코드 생성 상태가 범위를 초과하였습니다.");
            RuleFor(x => x.SearchType)
                .NotNull().InclusiveBetween(1, 3).WithMessage("검색 타입이 범위를 초과하였습니다.");
        }
    }

    public class GetHospitalAdminListQueryHandler : IRequestHandler<GetHospitalAdminListQuery, Result<ListResult<GetHospitalAdminListResult>>>
    {
        private readonly ILogger<GetHospitalAdminListQueryHandler> _logger;
        private readonly IAdminUserStore _adminUserStore;
        private readonly IDbSessionRunner _db;

        public GetHospitalAdminListQueryHandler(
            ILogger<GetHospitalAdminListQueryHandler> logger,
            IAdminUserStore adminUserStore,
            IDbSessionRunner db)
        {
            _logger = logger;
            _adminUserStore = adminUserStore;
            _db = db;
        }

        public async Task<Result<ListResult<GetHospitalAdminListResult>>> Handle(GetHospitalAdminListQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetHospitalAdminListQueryHandler");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _adminUserStore.GetHospitalAdminListAsync(session, req.PageNo, req.PageSize, req.QrState, req.SearchType, req.SearchKeyword, token), 
            ct);

            var test = result.ToJsonForStorage();

            return Result.Success(result);
        }
    }
}
