using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Advertisement.Commands
{
    public record UpdateEghisBannerCommand : IQuery<Result>
    {
        /// <summary>
        /// 광고 ID
        /// </summary>
        public int AdId { get; init; }
        /// <summary>
        /// 이미지 ID
        /// </summary>
        public int ImgId { get; init; }
        /// <summary>
        /// 노출여부
        /// </summary>
        public required string ShowYn { get; init; } = default!;
        /// <summary>
        /// 송신 타입 [A: 안드로이드, I: 아이폰, 0: 전체]
        /// </summary>
        public required string SendType { get; init; } = default!;
        /// <summary>
        /// 링크구분 [O: 외부, I: 내부, M: 메뉴이동, N: 링크없음]
        /// </summary>
        public required string LinkType { get; init; } = default!;
        /// <summary>
        /// 링크경로
        /// </summary>
        public string? Url { get; init; }
        /// <summary>
        /// 링크경로
        /// </summary>
        public string? Url2 { get; init; }
        /// <summary>
        /// 기간설정시작일자 (yyyy-mm-dd)
        /// </summary>
        public string? StartDt { get; init; }
        /// <summary>
        /// 기간설정만료일자 (yyyy-mm-dd)
        /// </summary>
        public string? EndDt { get; init; }
        /// <summary>
        /// 정렬 순서
        /// </summary>
        public required int SortNo { get; init; }
        /// <summary>
        /// 이미지 파일 (이지스 배너 이미지)
        /// </summary>
        public FileUploadPayload? ImagePayload { get; init; }
    }

    public class UpdateEghisBannerCommandHandler : IRequestHandler<UpdateEghisBannerCommand, Result>
    {
        private readonly ILogger<UpdateEghisBannerCommandHandler> _logger;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ISftpClientService _sftpClientService;
        private readonly IDbSessionRunner _db;

        public UpdateEghisBannerCommandHandler(
            ILogger<UpdateEghisBannerCommandHandler> logger,
            IAdvertisementRepository advertisementRepository,
            ISftpClientService sftpClientService,
            IDbSessionRunner db)
        {
            _logger = logger;
            _advertisementRepository = advertisementRepository;
            _sftpClientService = sftpClientService;
            _db = db;
        }

        public async Task<Result> Handle(UpdateEghisBannerCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpdateEghisBannerCommand");

            string imagePath = (req.ImagePayload != null)
                               ? await _sftpClientService.UploadImageWithPathAsync(req.ImagePayload, ImageUploadType.BA, "SuperAdmin", ct)
                               : string.Empty;

            var adInfoEntity = req.Adapt<TbAdInfoEntity>();
            adInfoEntity.AdType = AdvertType.BA.ToString();

            var imageEntity = new TbImageInfoEntity()
            {
                ImgId = req.ImgId,
                Url = imagePath
            };

            await _db.RunAsync(DataSource.Hello100,
                (dbSession, token) => _advertisementRepository.UpdateEghisBannerAsync(dbSession, adInfoEntity, imageEntity, token)
            , ct);

            // 이전 이미지 삭제는??

            return Result.Success();
        }
    }
}
