using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Notice.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Notice.Queries
{
    /// <summary>
    /// 공지사항 목록 조회 쿼리
    /// </summary>
    /// <param name="PageNo"></param>
    /// <param name="PageSize"></param>
    /// <param name="SearchKeyword"></param>
    public record GetNoticesQuery(int PageNo, int PageSize, string? SearchKeyword) : IQuery<Result<ListResult<GetNoticesResult>>>;

    public class GetNoticesQueryValidator : AbstractValidator<GetNoticesQuery>
    {
        public GetNoticesQueryValidator()
        {
            RuleFor(x => x.PageNo)
                .NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize)
                .NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
        }
    }

    public class GetNoticesQueryHandler : IRequestHandler<GetNoticesQuery, Result<ListResult<GetNoticesResult>>>
    {
        private readonly ILogger<GetNoticesQueryHandler> _logger;
        private readonly INoticeStore _noticeStore;
        private readonly IDbSessionRunner _db;

        public GetNoticesQueryHandler(ILogger<GetNoticesQueryHandler> logger, INoticeStore noticeStore, IDbSessionRunner db)
        {
            _logger = logger;
            _noticeStore = noticeStore;
            _db = db;
        }

        public async Task<Result<ListResult<GetNoticesResult>>> Handle(GetNoticesQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handle GetNoticesQueryHandler");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _noticeStore.GetNoticesAsync(session, req.PageNo, req.PageSize, req.SearchKeyword, token),
            ct);

            return Result.Success(result);
        }
    }
}
