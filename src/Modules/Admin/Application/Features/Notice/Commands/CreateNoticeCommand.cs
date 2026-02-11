using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Notice.Commands
{
    public record CreateNoticeCommand(string Title, string Content, string SendType, string ShowYn) : IQuery<Result>;

    public class CreateNoticeCommandHandler : IRequestHandler<CreateNoticeCommand, Result>
    {
        private readonly ILogger<CreateNoticeCommandHandler> _logger;
        private readonly INoticeRepository _noticeRepository;
        private readonly IDbSessionRunner _db;

        public CreateNoticeCommandHandler(
            ILogger<CreateNoticeCommandHandler> logger, 
            INoticeRepository noticeRepository, 
            IDbSessionRunner db) 
        {
            _logger = logger;
            _noticeRepository = noticeRepository;
            _db = db;
        }

        public async Task<Result> Handle(CreateNoticeCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handle CreateNoticeCommandHandler");

            var noticeEntity = new TbNoticeEntity()
            {
                Title = req.Title,
                Content = req.Content,
                SendType = req.SendType,
                ShowYn = req.ShowYn
            };

            await _db.RunAsync(DataSource.Hello100, 
                (session, token) => _noticeRepository.CreateNoticeAsync(session, noticeEntity, token),
            ct);

            return Result.Success();
        }
    }
}
