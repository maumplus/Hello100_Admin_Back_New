using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Notice.Commands
{
    public record UpdateNoticeCommand(int NotiId, string Title, string Content, string SendType, string ShowYn) : IQuery<Result>;

    public class UpdateNoticeCommandValidator : AbstractValidator<UpdateNoticeCommand>
    {
        public UpdateNoticeCommandValidator()
        {
            RuleFor(x => x.NotiId)
                .NotNull().GreaterThan(0).WithMessage("공지 ID는 필수이며 0보다 커야 합니다.");
        }
    }

    public class UpdateNoticeCommandHandler : IRequestHandler<UpdateNoticeCommand, Result>
    {
        private readonly ILogger<UpdateNoticeCommandHandler> _logger;
        private readonly INoticeRepository _noticeRepository;
        private readonly IDbSessionRunner _db;

        public UpdateNoticeCommandHandler(
            ILogger<UpdateNoticeCommandHandler> logger,
            INoticeRepository noticeRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _noticeRepository = noticeRepository;
            _db = db;
        }

        public async Task<Result> Handle(UpdateNoticeCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handle UpdateNoticeCommandHandler");

            var noticeEntity = new TbNoticeEntity()
            {
                NotiId = req.NotiId,
                Title = req.Title,
                Content = req.Content,
                SendType = req.SendType,
                ShowYn = req.ShowYn
            };

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _noticeRepository.UpdateNoticeAsync(session, noticeEntity, token),
            ct);

            return Result.Success();
        }
    }
}
