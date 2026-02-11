using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Advertisement.Commands
{
    public record BulkUpdateEghisBannersCommand(List<BulkUpdateEghisBannersCommandItem> Items) : IQuery<Result>;

    public record BulkUpdateEghisBannersCommandItem
    {
        public int AdId { get; set; }
        public string ShowYn { get; set; } = default!;
        public int SortNo { get; set; }
    }

    public class BulkUpdateEghisBannersCommandHandler : IRequestHandler<BulkUpdateEghisBannersCommand, Result>
    {
        private readonly ILogger<BulkUpdateEghisBannersCommandHandler> _logger;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IDbSessionRunner _db;

        public BulkUpdateEghisBannersCommandHandler(
            ILogger<BulkUpdateEghisBannersCommandHandler> logger, 
            IAdvertisementRepository advertisementRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _advertisementRepository = advertisementRepository;
            _db = db;
        }

        public async Task<Result> Handle(BulkUpdateEghisBannersCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handle BulkUpdateEghisBannersCommandHandler");

            var entities = req.Items.Adapt<List<TbAdInfoEntity>>();

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _advertisementRepository.BulkUpdateEghisBannersAsync(session, entities, token),
            ct);

            return Result.Success();
        }
    }
}
