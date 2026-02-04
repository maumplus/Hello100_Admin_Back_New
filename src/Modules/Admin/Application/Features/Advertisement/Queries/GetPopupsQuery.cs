using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Advertisement.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Advertisement.Queries
{
    public record GetPopupsQuery(int PageNo, int PageSize) : IQuery<Result<ListResult<GetPopupsResult>>>;

    public class GetPopupsQueryValidator : AbstractValidator<GetPopupsQuery>
    {
        public GetPopupsQueryValidator()
        {
            RuleFor(x => x.PageNo)
                .NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize)
                .NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
        }
    }

    public class GetPopupsQueryHandler : IRequestHandler<GetPopupsQuery, Result<ListResult<GetPopupsResult>>>
    {
        private readonly string _adminImageUrl;
        private readonly ILogger<GetPopupsQueryHandler> _logger;
        private readonly IAdvertisementStore _advertisementStore;
        private readonly IDbSessionRunner _db;

        public GetPopupsQueryHandler(IConfiguration config, ILogger<GetPopupsQueryHandler> logger, IAdvertisementStore advertisementStore, IDbSessionRunner db)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _logger = logger;
            _advertisementStore = advertisementStore;
            _db = db;
        }

        public async Task<Result<ListResult<GetPopupsResult>>> Handle(GetPopupsQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetPopupsQuery: PageNo={PageNo}, PageSize={PageSize}", req.PageNo, req.PageSize);

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _advertisementStore.GetPopupsAsync(session, req.PageNo, req.PageSize, token),
                ct);

            foreach (var item in result.Items)
            {
                item.ImgUrl = (string.IsNullOrEmpty(item.ImgUrl) == false) ? $"{_adminImageUrl}{item.ImgUrl}" : string.Empty;
            }

            return Result.Success(result);
        }
    }
}
