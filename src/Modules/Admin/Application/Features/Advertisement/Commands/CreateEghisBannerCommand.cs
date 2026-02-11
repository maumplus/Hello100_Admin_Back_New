using FluentValidation;
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
    public record CreateEghisBannerCommand : IQuery<Result>
    {
        /// <summary>
        /// 노출여부
        /// </summary>
        public string ShowYn { get; init; } = default!;
        /// <summary>
        /// 송신 타입 [A: 안드로이드, I: 아이폰, 0: 전체]
        /// </summary>
        public string SendType { get; init; } = default!;
        /// <summary>
        /// 링크구분 [O: 외부, I: 내부, M: 메뉴이동, N: 링크없음]
        /// </summary>
        public string LinkType { get; init; } = default!;
        /// <summary>
        /// 링크경로
        /// </summary>
        public string? Url { get; init; }
        /// <summary>
        /// 메뉴링크경로
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
        public int SortNo { get; init; }
        /// <summary>
        /// 이미지 파일 (이지스 배너 이미지)
        /// </summary>
        public FileUploadPayload? ImagePayload { get; init; }
    }

    public class CreateEghisBannerCommandValidator : AbstractValidator<CreateEghisBannerCommand>
    {
        public CreateEghisBannerCommandValidator()
        {
            RuleFor(x => x.ImagePayload)
                .NotNull().WithMessage("이미지 파일은 필수입니다.");
        }
    }

    public class CreateEghisBannerCommandHandler : IRequestHandler<CreateEghisBannerCommand, Result>
    {
        private readonly ILogger<CreateEghisBannerCommandHandler> _logger;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ISftpClientService _sftpClientService;
        private readonly IDbSessionRunner _db;

        public CreateEghisBannerCommandHandler(
            ILogger<CreateEghisBannerCommandHandler> logger,
            IAdvertisementRepository advertisementRepository,
            ISftpClientService sftpClientService,
            IDbSessionRunner db)
        {
            _logger = logger;
            _advertisementRepository = advertisementRepository;
            _sftpClientService = sftpClientService;
            _db = db;
        }

        public async Task<Result> Handle(CreateEghisBannerCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling CreatePopupCommand");

            string imagePath = await _sftpClientService.UploadImageWithPathAsync(req.ImagePayload!, ImageUploadType.BA, "SuperAdmin", ct);

            var adInfoEntity = req.Adapt<TbAdInfoEntity>();
            adInfoEntity.AdType = AdvertType.BA.ToString();

            var imageEntity = new TbImageInfoEntity()
            {
                Url = imagePath
            };

            await _db.RunAsync(DataSource.Hello100,
                (dbSession, ct) => _advertisementRepository.CreateAdvertisementAsync(dbSession, adInfoEntity, imageEntity, ct)
            , ct);

            return Result.Success();
        }
    }
}
