using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Advertisement.Queries
{
    public record GetEghisBannersQuery : IQuery<Result<ListResult<GetEghisBannersResult>>>;

    public class GetEghisBannersQueryHandler : IRequestHandler<GetEghisBannersQuery, Result<ListResult<GetEghisBannersResult>>>
    {
        private readonly string _adminImageUrl;
        private readonly ILogger<GetEghisBannersQueryHandler> _logger;
        private readonly IAdvertisementStore _advertisementStore;
        private readonly IDbSessionRunner _db;

        public GetEghisBannersQueryHandler(IConfiguration config, ILogger<GetEghisBannersQueryHandler> logger, IAdvertisementStore advertisementStore, IDbSessionRunner db)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _logger = logger;
            _advertisementStore = advertisementStore;
            _db = db;
        }

        public async Task<Result<ListResult<GetEghisBannersResult>>> Handle(GetEghisBannersQuery request, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetEghisBannersQuery");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _advertisementStore.GetEghisBannersAsync(session, token),
            ct);

            if (result.Items.Count > 0)
            {
                foreach (var item in result.Items)
                {
                    item.ImgUrl = $"{_adminImageUrl}{item.ImgUrl}";
                }
            }

            return Result.Success(result);
        }
    }
}
