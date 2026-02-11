using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Notice.Commands
{
    public record DeleteNoticeCommand(int NotiId) : IQuery<Result>;

    public class DeleteNoticeCommandValidator : AbstractValidator<DeleteNoticeCommand>
    {
        public DeleteNoticeCommandValidator()
        {
            RuleFor(x => x.NotiId)
                .NotNull().GreaterThan(0).WithMessage("공지 ID는 필수이며 0보다 커야 합니다.");
        }
    }

    public class DeleteNoticeCommandHandler : IRequestHandler<DeleteNoticeCommand, Result>
    {
        private readonly ILogger<DeleteNoticeCommandHandler> _logger;
        private readonly INoticeRepository _noticeRepository;
        private readonly IDbSessionRunner _db;

        public DeleteNoticeCommandHandler(
            ILogger<DeleteNoticeCommandHandler> logger,
            INoticeRepository noticeRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _noticeRepository = noticeRepository;
            _db = db;
        }

        public async Task<Result> Handle(DeleteNoticeCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handle DeleteNoticeCommandHandler");

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _noticeRepository.DeleteNoticeAsync(session, req.NotiId, token),
            ct);

            return Result.Success();
        }
    }
}
