using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Advertisement.Commands
{
    /// <summary>
    /// 팝업 광고 삭제 Command
    /// </summary>
    /// <param name="AdId">광고 ID</param>
    public record DeletePopupCommand(int AdId) : IQuery<Result>;

    public class DeletePopupCommandValidator : AbstractValidator<DeletePopupCommand>
    {
        public DeletePopupCommandValidator()
        {
            RuleFor(x => x.AdId)
                .NotNull().GreaterThan(0).WithMessage("유효한 광고 ID를 입력해주세요.");
        }
    }

    public class DeletePopupCommandHandler : IRequestHandler<DeletePopupCommand, Result>
    {
        private readonly ILogger<DeletePopupCommandHandler> _logger;
        private readonly IAdvertisementStore _advertisementStore;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ISftpClientService _sftpClientService;
        private readonly IDbSessionRunner _db;

        public DeletePopupCommandHandler(ILogger<DeletePopupCommandHandler> logger, 
                                         IAdvertisementStore advertisementStore, 
                                         IAdvertisementRepository advertisementRepository,
                                         ISftpClientService sftpClientService,
                                         IDbSessionRunner db)
        {
            _logger = logger;
            _advertisementStore = advertisementStore;
            _advertisementRepository = advertisementRepository;
            _sftpClientService = sftpClientService;
            _db = db;
        }

        public async Task<Result> Handle(DeletePopupCommand req, CancellationToken ct)
        {
            _logger.LogInformation("DeletePopupCommandHandler Handle Start [{AdId}]", req.AdId);

            var popupInfo = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _advertisementStore.GetAdvertisementAsync(session, req.AdId, token),
                ct);

            var adEntity = new TbAdInfoEntity
            {
                AdId = req.AdId,
                AdType = ImageUploadType.PO.ToString()
            };

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _advertisementRepository.DeleteAdvertisementAsync(session, adEntity, token),
                ct);

            await _sftpClientService.DeleteFileAsync(popupInfo.ImgUrl, ct);

            return Result.Success();
        }
    }
}
