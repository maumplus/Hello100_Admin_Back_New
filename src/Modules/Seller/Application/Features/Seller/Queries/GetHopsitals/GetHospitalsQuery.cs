using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Application.Features.Seller.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetHopsitals
{
    public record GetHospitalsQuery(string SearchText) : IQuery<Result<List<GetHospitalsResult>>>;

    public class GetHospitalsQueryValidator : AbstractValidator<GetHospitalsQuery>
    {
        public GetHospitalsQueryValidator()
        {
            RuleFor(x => x.SearchText)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("검색어는 빈값이 될 수 없습니다.");
        }
    }

    public class GetHospitalsQueryHandler : IRequestHandler<GetHospitalsQuery, Result<List<GetHospitalsResult>>>
    {
        private readonly ILogger<GetHospitalsQueryHandler> _logger;
        private readonly ISellerStore _sellerStore;
        private readonly IDbSessionRunner _db;

        public GetHospitalsQueryHandler(ILogger<GetHospitalsQueryHandler> logger, ISellerStore sellerStore, IDbSessionRunner db)
        {
            _logger = logger;
            _sellerStore = sellerStore;
            _db = db;
        }

        public async Task<Result<List<GetHospitalsResult>>> Handle(GetHospitalsQuery request, CancellationToken ct)
        {
            _logger.LogInformation("GetHospitalsQueryHandler Handle Start");

            var result = await _db.RunAsync(DataSource.Hello100, 
                (session, token) => _sellerStore.GetHospitalsAsync(session, request.SearchText, token),
            ct);

            return Result.Success(result);
        }
    }
}
