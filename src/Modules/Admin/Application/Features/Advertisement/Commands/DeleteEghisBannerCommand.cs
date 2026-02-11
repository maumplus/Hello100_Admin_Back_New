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
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Advertisement.Commands
{
    public record DeleteEghisBannerCommand(int AdId) : IQuery<Result>;

    public class DeleteEghisBannerCommandValidator : AbstractValidator<DeleteEghisBannerCommand>
    {
        public DeleteEghisBannerCommandValidator() 
        {
            RuleFor(x => x.AdId)
                .NotNull().GreaterThan(0).WithMessage("광고 ID는 필수이며 0보다 커야 합니다.");
        }
    }
    
    public class DeleteEghisBannerCommandHandler : IRequestHandler<DeleteEghisBannerCommand, Result>
    {
        private readonly ILogger<DeleteEghisBannerCommandHandler> _logger;
        private readonly IAdvertisementStore _advertisementStore;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ISftpClientService _sftpClientService;
        private readonly IDbSessionRunner _db;

        public DeleteEghisBannerCommandHandler(ILogger<DeleteEghisBannerCommandHandler> logger,
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

        public async Task<Result> Handle(DeleteEghisBannerCommand req, CancellationToken ct)
        {
            _logger.LogInformation("DeleteEghisBannerCommandHandler Handle Start [{AdId}]", req.AdId);

            var bannerInfo = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _advertisementStore.GetAdvertisementAsync(session, req.AdId, token),
                ct);

            var adEntity = new TbAdInfoEntity
            {
                AdId = req.AdId,
                AdType = ImageUploadType.BA.ToString()
            };

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _advertisementRepository.DeleteAdvertisementAsync(session, adEntity, token),
                ct);

            await _sftpClientService.DeleteFileAsync(bannerInfo.ImgUrl, ct);

            // 다른 애들 정렬 순서는??

            return Result.Success();
        }
    }
}
