using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Features.Notice.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Notice.Queries
{
    public record GetNoticeQuery(int NotiId) : IQuery<Result<GetNoticeResult>>;

    public class GetNoticeQueryValidator : AbstractValidator<GetNoticeQuery>
    {
        public GetNoticeQueryValidator() 
        {
            RuleFor(x => x.NotiId)
                .NotNull().GreaterThan(0).WithMessage("공지 ID는 필수이며 0보다 커야 합니다.");
        }
    }

    public class GetNoticeQueryHandler : IRequestHandler<GetNoticeQuery, Result<GetNoticeResult>>
    {
        private readonly ILogger<GetNoticeQueryHandler> _logger;
        private readonly INoticeStore _noticeStore;
        private readonly IDbSessionRunner _db;

        public GetNoticeQueryHandler(ILogger<GetNoticeQueryHandler> logger, INoticeStore noticeStore, IDbSessionRunner db)
        {
            _logger = logger;
            _noticeStore = noticeStore;
            _db = db;
        }

        public async Task<Result<GetNoticeResult>> Handle(GetNoticeQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handle GetNoticesQueryHandler");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _noticeStore.GetNoticeAsync(session, req.NotiId, token),
            ct);

            return Result.Success(result);
        }
    }
}
