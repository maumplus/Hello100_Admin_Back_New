using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Advertisement.Queries
{
    public record GetPopupQuery(int PopupId) : IQuery<Result<GetPopupResult>>;

    public class GetPopupQueryValidator : AbstractValidator<GetPopupQuery>
    {
        public GetPopupQueryValidator()
        {
            RuleFor(x => x.PopupId)
                .NotNull().GreaterThan(0).WithMessage("광고 ID는 필수이며 0보다 커야 합니다.");
        }
    }

    public class GetPopupQueryHandler : IRequestHandler<GetPopupQuery, Result<GetPopupResult>>
    {
        private readonly string _adminImageUrl;
        private readonly ILogger<GetPopupQueryHandler> _logger;
        private readonly IAdvertisementStore _advertisementStore;
        private readonly IDbSessionRunner _db;

        public GetPopupQueryHandler(IConfiguration config, ILogger<GetPopupQueryHandler> logger, IAdvertisementStore advertisementStore, IDbSessionRunner db)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _logger = logger;
            _advertisementStore = advertisementStore;
            _db = db;
        }

        public async Task<Result<GetPopupResult>> Handle(GetPopupQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetPopupQuery: PopupId={PopupId}", req.PopupId);

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _advertisementStore.GetAdvertisementAsync(session, req.PopupId, token),
                ct);

            result.ImgUrl = $"{_adminImageUrl}{result.ImgUrl}";

            return Result.Success(result);
        }
    }
}
